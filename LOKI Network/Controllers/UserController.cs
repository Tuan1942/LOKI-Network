using Azure.Identity;
using LOKI_Model.Models;
using LOKI_Network.Helpers;
using LOKI_Network.Interface;
using LOKI_Network.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LOKI_Network.Controllers
{
    [Route("[controller]")]
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
        public async Task<IActionResult> Register([FromBody] UserDTO user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var u = await _userService.GetUser(user.Username);
                if (u != null)
                {
                    return Problem("Username already existed.");
                }

                await _userService.AddUser(user);

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
            try
            {
                if (user == null || !_userService.VerifyPassword(user))
                {
                    return Unauthorized();
                }
                var u = await _userService.GetUser(user.Username);
                var jwtHelper = new JwtHelper(_configuration);
                var result = jwtHelper.GenerateJwtToken(u, _configuration);
                u.Token = result.Item1;
                u.TokenExpirationDate = result.Item2;
                return Ok(u);
            }
            catch (Exception e)
            {
                return Problem(e.Message);
            }
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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _userService.GetAll());
        }
    }
}
