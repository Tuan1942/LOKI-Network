using LOKI_Client.ApiClients.Interfaces;
using LOKI_Client.ApiClients.Services;
using LOKI_Client.UIs.ViewModels.Account;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace LOKI_Client.Extensions
{
    public static class ServiceExtension
    {
        public static void InjectDependencies(this IServiceCollection services)
        {
            // Register ViewModels
            services.AddSingleton<LoginViewModel>();

            // Configure HttpClient
            services.AddHttpClient("LokiClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:3000/"); // Update to your API base URL
            });

            // Register Services
            services.AddSingleton<IUserService, UserService>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var client = httpClientFactory.CreateClient("LokiClient");
                return new UserService(client);
            });
            //services.AddTransient<MainWindow>();
        }
    }
}
