using System.Windows;
using Billiard.Physics;
using Billiard.viewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Billiard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static ServiceProvider? _serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<MainWindow>();

            services.AddSingleton<FilterViewModel>();
            services.AddSingleton<VideoDeviceViewModel>();
            services.AddSingleton<BallViewModel>();
            services.AddSingleton<TableViewModel>();
            services.AddSingleton<CaptureViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<PhysicsEngine>();

/*              services.AddSingleton<DataViewModel>();
                                                                                        services.AddSingleton<TranslatorViewModel>();
                                                                                        services.AddSingleton<ImagesViewModel>();
                                                                                        services.AddSingleton<PdfViewModel>();
                                                                                        services.AddSingleton<EnumUpdater>();
                                                                                    */
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }

        public static T GetService<T>()
        {
            return _serviceProvider.GetService<T>();
        }
    }
}
