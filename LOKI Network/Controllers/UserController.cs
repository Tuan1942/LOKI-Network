using Azure.Identity;
using LOKI_Network.DbContexts;
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
        private readonly LokiContext _context;
        private readonly IConfiguration _configuration;

        public UserController(LokiContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            { 
                return Problem("Username already existed."); 
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { user.Username });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User lUser)
        {
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == lUser.Username);
            if (dbUser == null || !VerifyPassword(lUser.PasswordHash, dbUser.PasswordHash))
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(dbUser, _configuration);
            return Ok(token);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var enteredHash = HashPassword(enteredPassword);
            return enteredHash == storedHash;
        }

        public string GenerateJwtToken(User user, IConfiguration configuration)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            //var userRoles = GetUserRoles(user.UserId);

            //foreach (var role in userRoles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, role));
            //}

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(16),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
