using LOKI_Network.DbContexts;
using LOKI_Network.DTOs;
using LOKI_Network.Interface;
using Microsoft.EntityFrameworkCore;

namespace LOKI_Network.Services
{
    public class FriendshipService : IFriendshipService
    {
        private readonly LokiContext _lokiContext;
        public FriendshipService(LokiContext lokiContext)
        {
            _lokiContext = lokiContext;
        }
        public async Task AddFriendRequest(Guid sender, Guid receiver)
        {
            // Check if any existing friendship already exists between sender and receiver
            var existingFriendships = await _lokiContext.Friendships
                .Where(f =>
                    (f.FriendId == sender && f.UserId == receiver) ||
                    (f.FriendId == receiver && f.UserId == sender))
                .ToListAsync();

            // Validate existing friendships
            if (existingFriendships.Any())
            {
                var blockedFriendship = existingFriendships.FirstOrDefault(f => f.Status == FriendshipStatus.Blocked);

                // Check if sender has blocked receiver
                if (blockedFriendship != null && blockedFriendship.UserId == sender)
                {
                    throw new InvalidOperationException("Please unblock this account before sending a friend request.");
                }

                // Check if receiver has blocked sender
                if (blockedFriendship != null && blockedFriendship.FriendId == sender)
                {
                    throw new InvalidOperationException("You have been blocked by this user.");
                }

                // Other non-blocked friendship already exists
                throw new InvalidOperationException("Friendship already exists.");
            }

            // If no friendship exists, create a new pending friend request
            var friendship = new Friendship
            {
                CreatedDate = DateTime.UtcNow,
                UserId = sender,
                FriendId = receiver,
                Status = FriendshipStatus.Pending,
            };

            _lokiContext.Friendships.Add(friendship);
            await _lokiContext.SaveChangesAsync();
        }

        public async Task ResponseFriendRequest(Guid sender, Guid friendShipId, bool accepted)
        {
            var friendship = _lokiContext.Friendships.FirstOrDefault(f => f.FriendshipId == friendShipId && f.UserId == sender);
            if (friendship == null) { throw new UnauthorizedAccessException(); }
            if (accepted)
            {
                friendship.Status = FriendshipStatus.Accepted;
                _lokiContext.Friendships.Update(friendship);
            }
            else _lokiContext.Friendships.Remove(friendship);
            await _lokiContext.SaveChangesAsync();
        }

        public async Task UnFriend(Guid sender, Guid receiver)
        {
            var query = _lokiContext.Friendships.Where(f => 
            (f.FriendshipId == sender && f.UserId == receiver) 
            || (f.FriendshipId == receiver && f.UserId == sender)
            || f.Status == FriendshipStatus.Accepted);
            _lokiContext.Friendships.RemoveRange(query);
            await _lokiContext.SaveChangesAsync();
        }

        public async Task<List<Friendship>> GetAllFriendRequests(Guid userId)
        {
            // Retrieve all pending friend requests where the user is the receiver
            return await _lokiContext.Friendships
                .Where(f => f.FriendId == userId && f.Status == FriendshipStatus.Pending)
                .ToListAsync();
        }

        public async Task<List<UserDTO>> GetAllFriends(Guid userId)
        {
            // Retrieve all accepted friendships where the user is either the sender or the receiver
            var userList = await _lokiContext.Friendships
                .Where(f =>
                (f.UserId == userId || f.FriendId == userId) &&
                f.Status == FriendshipStatus.Accepted)
                .Select(f => f.UserId == userId ? f.Friend : f.User) // Selects the friend based on relationship
                .ToListAsync();
            var userDtoList = userList.Select(user => new UserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Gender = user.Gender

            });
            return userDtoList.ToList();
        }

        public async Task UnBlock(Guid sender, Guid receiver)
        {
            throw new NotImplementedException();
        }

        public async Task Block(Guid sender, Guid receiver)
        {
            throw new NotImplementedException();
        }
    }
}
