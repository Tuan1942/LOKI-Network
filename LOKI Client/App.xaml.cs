using LOKI_Client.Dictionary;
using LOKI_Client.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace LOKI_Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new static App Current => (App)Application.Current;
        public IServiceProvider Services { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Services = ConfigureServices();
            LanguageManager.Instance.SetLanguage("en-US");
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.InJectDependencies();
            return services.BuildServiceProvider();
        }
    }

}
