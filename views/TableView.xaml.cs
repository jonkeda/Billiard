using Billiard.viewModels;

namespace Billiard.views
{
    public partial class TableView
    {
        public TableView()
        {
            InitializeComponent();

            DataContext = new TableViewModel();
        }
    }
}
