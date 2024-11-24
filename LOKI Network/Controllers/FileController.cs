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
        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var fileType = FileHelper.GetFileType(file.ContentType);
            var result = await _fileService.UploadFileAsync(file, fileType);
            if (result == null)
            {
                return StatusCode(500, "An error occurred while uploading the file.");
            }

            return Ok(new { FileUrl = result });
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> GetFile(string fileId)
        {
            var filePath = await _fileService.GetFileUrl(Guid.Parse(fileId));

            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = FileHelper.GetContentType(filePath);

            return File(fileBytes, contentType);
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
    }
}
