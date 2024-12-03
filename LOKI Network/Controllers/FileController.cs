using LOKI_Model.Enums;
using LOKI_Network.Helpers;
using LOKI_Network.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LOKI_Network.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }
        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var fileType = FileHelper.GetFileType(file.ContentType);
            var result = await _fileService.UploadFileAsync(file);
            if (result.FilePath == null)
            {
                return StatusCode(500, "An error occurred while uploading the file.");
            }

            return Ok(new { FileUrl = result });
        }

        [HttpGet("{fileId:guid}/download")]
        public async Task<IActionResult> GetFile(Guid fileId)
        {
            var file = await _fileService.GetFileUrl(fileId);
            var filePath = file.FileUrl;

            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = FileHelper.GetContentType(filePath);

            return File(fileBytes, contentType);
        }

        [HttpGet("{fileId:guid}")]
        public async Task<IActionResult> GetFile(Guid fileId, int quality = 20)
        {
            quality = Math.Max(0, Math.Min(100, quality));
            var file = await _fileService.GetFileUrl(fileId);
            var filePath = file.FileUrl;
            var fileType = file.FileType;
            if (fileType == FileType.Image)
            {
                if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found.");
                }

                using var originalImage = System.Drawing.Image.FromFile(filePath);
                using var stream = new MemoryStream();

                // Reduce the quality of the image
                var encoderParameters = new System.Drawing.Imaging.EncoderParameters(1);
                var qualityParameter = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality); // 50% quality
                encoderParameters.Param[0] = qualityParameter;

                // Get the image codec for JPEG
                var codec = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()
                                .FirstOrDefault(c => c.FormatID == System.Drawing.Imaging.ImageFormat.Jpeg.Guid);

                // Save the reduced-quality image to the stream
                originalImage.Save(stream, codec, encoderParameters);

                // Reset the stream position for the response
                stream.Position = 0;

                return File(stream.ToArray(), "image/jpeg");
            }
            else
            {
                var previewPath = FilePreview(filePath);
                var fileBytes = System.IO.File.ReadAllBytes(previewPath);
                var contentType = FileHelper.GetContentType(previewPath);

                return File(fileBytes, contentType);
            }
        }
        //[HttpDelete("{fileId}")]
        //public async Task<IActionResult> DeleteFile(string fileId)
        //{
        //    var filePath = await _fileService.GetFileUrl(Guid.Parse(fileId));
        //    var isDeleted = _fileService.DeleteFile(filePath);
        //    if (!isDeleted)
        //    {
        //        return NotFound("File not found or couldn't be deleted.");
        //    }

        //    return Ok("File deleted successfully.");
        //}
        private string FilePreview(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            // For image files
            if (extension is ".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif" or ".jfif")
            {
                return "";
            }
            // For video files
            else if (extension is ".mp4" or ".mkv" or ".avi" or ".mov" or ".wmv")
            {
                return "Resources/FileIcons/video_icon.png";
            }
            // For audio files
            else if (extension is ".mp3" or ".wav" or ".ogg" or ".aac" or ".flac")
            {
                return "Resources/FileIcons/audio_icon.png";
            }
            // For word files
            else if (extension is ".doc" or ".docx")
            {
                return "Resources/FileIcons/doc_icon.png";
            }
            // For excel files
            else if (extension is ".xlsx" or ".xls")
            {
                return "Resources/FileIcons/xls_icon.png";
            }
            // For power point files
            else if (extension is ".ppt" or ".pptx")
            {
                return "Resources/FileIcons/ppt_icon.png";
            }
            // For pdf files
            else if (extension is ".pdf")
            {
                return "Resources/FileIcons/pdf_icon.png";
            }
            // Default for other file types
            else
            {
                return "Resources/FileIcons/oth_icon.png";
            }
        }
    }
}
