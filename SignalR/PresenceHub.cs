using LearnerDuo.Extentions;
using LearnerDuo.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LearnerDuo.SignalIR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;
        public PresenceHub(PresenceTracker presenceTracker)
        {
            _presenceTracker = presenceTracker;
        }
        public override async Task OnConnectedAsync()
        {
            bool isOnline = await _presenceTracker.UserConnected(Context.User.GetUserName(), Context.User.GetUserId().ToString());

            if (isOnline) await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());

            var currentUsers = await _presenceTracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            bool isOffline = await _presenceTracker.UserDisconnected(Context.User.GetUserName(), Context.User.GetUserId().ToString());

            if (isOffline) await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());

            await base.OnDisconnectedAsync(exception);
        }
    }
}
