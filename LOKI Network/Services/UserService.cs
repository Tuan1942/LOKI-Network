using LOKI_Network.DbContexts;
using LOKI_Network.DTOs;
using LOKI_Network.Interface;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LOKI_Network.Services
{

    public class UserService : IUserService
    {
        private readonly LokiContext _context;

        public UserService(LokiContext context)
        {
            _context = context;
        }

        public async Task AddUser(UserDTO user)
        {
            try
            {
                var u = new User
                {
                    Username = user.Username,
                    PasswordHash = HashPassword(user.Password),
                    Email = user.Email,
                    Gender = user.Gender,
                    CreatedDate = DateTime.Now
                };
                _context.Users.Add(u);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task UpdateUser(UserDTO user)
        {
            var u = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (u == null) throw new KeyNotFoundException();
            u.Username = user.Username;
            u.Gender = user.Gender;
            u.Email = user.Email;
            u.PasswordHash = HashPassword(user.Password);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUser(Guid userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null) throw new KeyNotFoundException();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUser(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUser(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public bool VerifyPassword(string enteredPassword, string storedHash)
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
        //private List<string> GetUserRoles(int userId)
        //{
        //    var roles = new List<string>();
        //    var userRoles = _context.UserRoles.Where(ur => ur.UserId == userId).ToList();
        //    foreach (var userRole in userRoles)
        //    {
        //        roles.Add(_context.Roles.First(r => r.Id == userRole.RoleId).Name);
        //    }
        //    return roles;
        //}

        public static async Task<User?> ValidateJwtToken(string token, IConfiguration configuration)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null; // Invalid token
                }

                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var usernameClaim = principal.FindFirst(ClaimTypes.Name)?.Value;

                if (Guid.TryParse(userIdClaim, out var userId) && !string.IsNullOrEmpty(usernameClaim))
                {
                    return await _context.Users.FindAsync(userIdClaim);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
