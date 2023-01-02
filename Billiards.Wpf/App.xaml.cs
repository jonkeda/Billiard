using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Billiard.Physics;
using Billiard.Threading;
using Billiard.viewModels;
using Billiards.Base.Threading;

namespace Billiards.Wpf
{
    public partial class App
    {
        private static readonly ServiceProvider ServiceProvider;

        static App()
        {
            ThreadDispatcher.Dispatcher = new WpfDispatcher();

            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<MainWindow>();

            services.AddSingleton<FilterViewModel>();
            services.AddSingleton<VideoDeviceViewModel>();
            services.AddSingleton<CaptureViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<PhysicsEngine>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow? mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow?.Show();
        }

        public static T? GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }
    }
}
