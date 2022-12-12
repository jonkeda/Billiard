using System.Windows.Controls;
using Billiard.viewModels;

namespace Billiard.views
{
    public partial class CaptureView
    {
        public CaptureView()
        {
            InitializeComponent();

            DataContext = new CaptureViewModel();
        }
    }
}
