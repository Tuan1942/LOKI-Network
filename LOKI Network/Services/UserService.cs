using LOKI_Network.DbContexts;
using LOKI_Network.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LOKI_Network.Services
{
    public interface IUserService
    {
        void AddUser(UserDTO user);
        void RemoveUser(Guid userId);
        void UpdateUser(UserDTO user);
        User GetUser(Guid id);
        User GetUser(string username);
        string HashPassword(string password);
        bool VerifyPassword(string enteredPassword, string storedHash);
        string GenerateJwtToken(User user, IConfiguration configuration);
    }

    public class UserService : IUserService
    {
        private readonly LokiContext _context;

        public UserService(LokiContext context)
        {
            _context = context;
        }

        public void AddUser(UserDTO user)
        {
            var u = new User
            {
                Username = user.Username,
                PasswordHash = HashPassword(user.PasswordHash),
                Email = user.Email,
                Gender = user.Gender,
                CreatedDate = DateTime.Now
            };
            _context.Users.Add(u);
            _context.SaveChanges();
        }

        public void UpdateUser(UserDTO user)
        {
            var u = _context.Users.FirstOrDefault(u => u.UserId == user.UserId);
            if (u == null) throw new KeyNotFoundException();
            u.Username = user.Username;
            u.Gender = user.Gender;
            u.Email = user.Email;
            u.PasswordHash = HashPassword(user.PasswordHash);
            _context.SaveChanges();
        }

        public void RemoveUser(Guid userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null) throw new KeyNotFoundException();
            _context.Users.Remove(user);
        }

        public User GetUser(Guid id)
        {
            return _context.Users.Find(id);
        }

        public User GetUser(string username)
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

    }
}
