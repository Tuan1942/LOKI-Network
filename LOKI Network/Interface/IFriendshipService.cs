﻿using LOKI_Network.DbContexts;

namespace LOKI_Network.Interface
{
    public interface IFriendshipService
    {
        Task AddFriendRequest(Guid sender, Guid receiver);
        Task ResponseFriendRequest(Guid sender, Guid friendShipId, bool accepted);
        Task UnFriend(Guid sender, Guid receiver);
        Task<List<Friendship>> GetAllFriendRequests(Guid userId);
        Task<List<User>> GetAllFriends(Guid userId);
        Task Block(Guid sender, Guid receiver);
        Task UnBlock(Guid sender, Guid receiver);
    }
}