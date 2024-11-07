namespace LOKI_Network.Interface
{
    public interface IFriendshipService
    {
        Task AddFriendRequest(Guid sender, Guid receiver);
        void ResponseFriendRequest(Guid sender, Guid friendShipId, bool accepted);
        void UnFriend(Guid sender, Guid receiver);
        void Block(Guid sender, Guid receiver);
        void UnBlock(Guid sender, Guid receiver);
    }
}
