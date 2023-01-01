using System.Windows.Input;
using Billiard.viewModels;
using Point = System.Windows.Point;

namespace Billiard.views
{
    public partial class FilterView
    {
        public FilterView()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is TableViewModel model)
            {
                Point p = e.GetPosition(resultImage);
                double x = p.X / resultImage.ActualWidth;
                double y = p.Y / resultImage.ActualHeight;

                model.ClickOnColorResult(x, y);
            }
        }
    }
}
