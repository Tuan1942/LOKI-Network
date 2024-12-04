using LOKI_Model.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace LOKI_Client.ApiClients.Services
{
    public class SignalRService
    {
        private readonly HubConnection _hubConnection;

        public SignalRService(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
        }

        public async Task StartAsync()
        {
            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }
        }

        public async Task StopAsync()
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.StopAsync();
            }
        }

        public void On<T>(string methodName, Action<T> handler)
        {
            _hubConnection.On(methodName, handler);
        }

        public async Task SendAsync(string methodName, object data)
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.SendAsync(methodName, data);
            }
        }
    }
}
