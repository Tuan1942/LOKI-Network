namespace LOKI_Network.Interface
{
    public interface IFriendshipService
    {
        Task AddFriendRequest(Guid sender, Guid receiver);
        Task ResponseFriendRequest(Guid sender, Guid friendShipId, bool accepted);
        Task UnFriend(Guid sender, Guid receiver);
        Task Block(Guid sender, Guid receiver);
        Task UnBlock(Guid sender, Guid receiver);
    }
}
