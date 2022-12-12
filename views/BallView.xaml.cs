using Billiard.viewModels;

namespace Billiard.views
{
    public partial class BallView
    {
        public BallView()
        {
            InitializeComponent();

            DataContext = new BallViewModel();
        }
    }
}
