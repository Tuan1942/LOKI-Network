using LOKI_Network.DbContexts;
using LOKI_Network.Interface;

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
            Friendship friendship = new Friendship()
            {
                CreatedDate = DateTime.Now,
                UserId = sender,
                FriendId = receiver,
                Status = 0,
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
            var query = _lokiContext.Friendships.Where(f => (f.FriendshipId == sender && f.UserId == receiver) || (f.FriendshipId == receiver && f.UserId == sender));
            foreach (var friendship in query)
            {
                _lokiContext.Friendships.Remove(friendship);
            }
            await _lokiContext.SaveChangesAsync();
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
