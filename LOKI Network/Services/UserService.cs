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

        public async Task<UserDTO> GetUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            return new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Gender = user.Gender
            };
        }

        public async Task<UserDTO> GetUser(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return null;
            return new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Gender = user.Gender
            };
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public bool VerifyPassword(UserDTO user)
        {
            var u = _context.Users.FirstOrDefault(u => u.Username == user.Username);
            if (u == null) return false;
            var enteredHash = HashPassword(user.Password);
            return enteredHash == u.PasswordHash;
        }

    }
}
