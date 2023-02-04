using System.Collections.ObjectModel;
using OpenCvSharp;

namespace Billiards.Base.Filters
{
    public class FilterSetCollection : ObservableCollection<FilterSet>
    {
        private readonly bool drawImage;

        public FilterSetCollection(bool drawImage)
        {
            this.drawImage = drawImage;
        }

        public T AddSet<T>(T set) where T : FilterSet
        {
            Add(set);
            return set;
        }

        protected override void InsertItem(int index, FilterSet item)
        {
            base.InsertItem(index, item);
            item.DrawImage = drawImage;
        }

        protected override void SetItem(int index, FilterSet item)
        {
            base.SetItem(index, item);
            item.DrawImage = drawImage;
        }

        public void ApplyFilters(Mat? image)
        {
            foreach (FilterSet set in this)
            {
                set.ApplyFilters(image);
            }
        }
    }
}
