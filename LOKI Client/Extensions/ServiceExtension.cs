using LOKI_Client.ApiClients.Interfaces;
using LOKI_Client.ApiClients.Services;
using LOKI_Client.Extensions.Authorize;
using LOKI_Client.Properties;
using LOKI_Client.UIs.ViewModels;
using LOKI_Client.UIs.ViewModels.Account;
using LOKI_Client.UIs.ViewModels.Conversation;
using LOKI_Client.UIs.ViewModels.Message;
using Microsoft.AspNetCore.SignalR.Client;
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
            // Register the TokenProvider
            services.AddSingleton<UserProvider>();

            // Register the AuthorizationHandler
            services.AddTransient<AuthorizationHandler>();

            // Configure HttpClient
            services.AddHttpClient("LokiClient", client =>
            {
                client.BaseAddress = new Uri(BaseClient.ServerAddress);
            }).AddHttpMessageHandler<AuthorizationHandler>();

            // Register services using the HttpClient
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

            // Register SignalR HubConnection
            services.AddSingleton(provider =>
            {
                var userProvider = provider.GetRequiredService<UserProvider>();
                return new HubConnectionBuilder()
                    .WithUrl(BaseClient.ChatHubAddress, options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(userProvider.GetToken());
                    })
                    .WithAutomaticReconnect()
                    .Build();
            });

            // Register SignalRService
            services.AddSingleton<SignalRService>();

            services.AddSingleton(new WebSocketService(new Uri(BaseClient.WebSocketAddress)));
        }
    }
}
