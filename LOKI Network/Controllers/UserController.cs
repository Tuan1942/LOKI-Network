using Azure.Identity;
using LOKI_Network.DbContexts;
using LOKI_Network.DTOs;
using LOKI_Network.Interface;
using LOKI_Network.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LOKI_Network.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDTO user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var u = _userService.GetUser(user.Username);
                if (u != null)
                {
                    return Problem("Username already existed.");
                }

                _userService.AddUser(user);

                return Ok(new { user.Username });
            }
            catch (Exception e)
            {
                return BadRequest("An error occur when register.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDTO user)
        {
            var u = _userService.GetUser(user.Username);
            if (u == null || !_userService.VerifyPassword(user.PasswordHash, u.PasswordHash))
            {
                return Unauthorized();
            }

            var token = "Bearer " + _userService.GenerateJwtToken(u, _configuration);
            return Ok(new { token });
        }

        [HttpGet("info")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = HttpContext.User.Identity.Name;
            var user = _userService.GetUser(Guid.Parse(userId));
            var roles = HttpContext.User.FindAll(ClaimTypes.Role)?.Select(c => c.Value).ToList();

            return Ok(user);
        }
    }
}
