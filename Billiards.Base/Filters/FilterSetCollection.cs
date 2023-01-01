using System.Collections.ObjectModel;
using OpenCvSharp;

namespace Billiards.Base.Filters
{
    public class FilterSetCollection : ObservableCollection<FilterSet>
    {
        public T AddSet<T>(T set) where T : FilterSet
        {
            Add(set);
            return set;
        }

        public void ApplyFilters(Mat image)
        {
            foreach (FilterSet set in this)
            {
                set.ApplyFilters(image);
            }
        }
    }
}
