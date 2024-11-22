using LOKI_Client.ApiClients.Interfaces;
using LOKI_Client.ApiClients.Services;
using LOKI_Client.UIs.ViewModels;
using LOKI_Client.UIs.ViewModels.Account;
using LOKI_Client.UIs.ViewModels.Conversation;
using LOKI_Client.UIs.ViewModels.Message;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace LOKI_Client.Extensions
{
    public static class ServiceExtension
    {
        public static void InjectDependencies(this IServiceCollection services)
        {
            RegisterViewModels(services);

            RegisterServices(services);
            //services.AddTransient<MainWindow>();
        }
        private static void RegisterViewModels(IServiceCollection services)
        {
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<ConversationViewModel>();
            services.AddSingleton<MessageViewModel>();
        }
        private static void RegisterServices(IServiceCollection services)
        {
            // Configure HttpClient
            services.AddHttpClient("LokiClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:3000/");
            });

            services.AddSingleton<IConversationService, ConversationService>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var client = httpClientFactory.CreateClient("LokiClient");
                return new ConversationService(client);
            });
            services.AddSingleton<IUserService, UserService>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var client = httpClientFactory.CreateClient("LokiClient");
                return new UserService(client);
            });
            services.AddSingleton<FriendshipService>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var client = httpClientFactory.CreateClient("LokiClient");
                return new FriendshipService(client);
            });
            services.AddSingleton(new WebSocketService(new Uri("wss://localhost:3000/ws")));
        }
    }
}
