using System.Collections.ObjectModel;
using Emgu.CV;

namespace Billiard.Models
{
    public class FilterSetCollection : ObservableCollection<FilterSet>
    {
        public void ApplyFilters(Mat image)
        {
            foreach (FilterSet set in this)
            {
                set.ApplyFilters(image);
            }
        }
    }
}
