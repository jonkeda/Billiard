using Billiard.viewModels;

namespace Billiards.Wpf
{
    public partial class MainWindow
    {
        private readonly MainViewModel mainViewModel;

        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();

            this.mainViewModel = mainViewModel;
            DataContext = mainViewModel;
        }
    }
}
