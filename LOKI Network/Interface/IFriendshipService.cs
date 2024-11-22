using LOKI_Model.Models;

namespace LOKI_Network.Interface
{
    public interface IFriendshipService
    {
        Task AddFriendRequest(Guid sender, Guid receiver);
        Task ResponseFriendRequest(Guid sender, Guid friendShipId, bool accepted);
        Task UnFriend(Guid sender, Guid receiver);
        Task<List<FriendshipDTO>> GetAllFriendRequests(Guid userId);
        Task<List<UserDTO>> GetAllFriends(Guid userId);
        Task Block(Guid sender, Guid receiver);
        Task UnBlock(Guid sender, Guid receiver);
    }
}
