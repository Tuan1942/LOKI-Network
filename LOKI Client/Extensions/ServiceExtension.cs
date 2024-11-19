using LOKI_Client.ApiClients.Interfaces;
using LOKI_Client.ApiClients.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace LOKI_Client.Extensions
{
    static class ServiceExtension
    {
        public static void InJectDependencies(this ServiceCollection services)
        {
            //services.AddSingleton<ThreatPreviewViewModel>();
            //services.AddSingleton<HeaderViewModel>();
            //services.AddSingleton<SettingViewModel>();
            //services.AddSingleton<ColorSettingViewModel>();
            //services.AddSingleton<HomeViewModel>();
            //services.AddSingleton<LogoutViewModel>();
            //services.AddSingleton<MainSettingViewModel>();
            //services.AddSingleton<LoginViewModel>();
            services.AddHttpClient("LokiClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:3000/");
            });
            services.AddSingleton<IUserService, UserService>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var client = httpClientFactory.CreateClient("LokiClient");
                return new UserService(client);
            });
        }
    }
}
