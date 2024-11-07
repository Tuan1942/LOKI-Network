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
            _lokiContext.SaveChangesAsync();
        }

        public void Block(Guid sender, Guid receiver)
        {
            throw new NotImplementedException();
        }

        public void ResponseFriendRequest(Guid sender, Guid friendShipId, bool accepted)
        {
            throw new NotImplementedException();
        }

        public void UnBlock(Guid sender, Guid receiver)
        {
            throw new NotImplementedException();
        }

        public void UnFriend(Guid sender, Guid receiver)
        {
            throw new NotImplementedException();
        }
    }
}
