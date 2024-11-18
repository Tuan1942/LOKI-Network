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
            services.AddHttpClient("DSApiClient", client =>
            {
                client.BaseAddress = new Uri("https://youtube.com");
            });
            //services.AddSingleton<ISettingService, SettingService>(provider =>
            //{
            //    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            //    var client = httpClientFactory.CreateClient("DSApiClient");
            //    return new SettingService(client);
            //});
        }
    }
}
