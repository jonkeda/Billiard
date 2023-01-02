using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Billiard.Threading;
using Billiard.viewModels;
using Billiards.Base.Physics;
using Billiards.Base.Threading;

namespace Billiards.Wpf
{
    public partial class App
    {
        private static ServiceProvider _serviceProvider;

        public App()
        {
            ThreadDispatcher.Dispatcher = new WpfDispatcher();

            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
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
            MainWindow? mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }

        public static T GetService<T>()
        {
            return _serviceProvider.GetService<T>();
        }
    }
}
