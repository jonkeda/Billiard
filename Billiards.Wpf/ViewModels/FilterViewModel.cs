using System.Linq;
using Billiards.Base.Filters;
using Billiards.Base.FilterSets;
using Billiards.Wpf.UI;

namespace Billiards.Wpf.ViewModels
{
    public class FilterViewModel : ViewModel
    {
        public VideoDeviceViewModel VideoDevice { get; }

        private AbstractFilter? selectedFilter;
        private FilterSet? selectedFilterSet;
        private FilterSetCollection? filterSets;

        public FilterSet? SelectedFilterSet
        {
            get { return selectedFilterSet; }
            set { SetProperty(ref selectedFilterSet, value); }
        }

        public FilterSetCollection? FilterSets
        {
            get { return filterSets; }
            set { SetProperty(ref filterSets, value); }
        }

        public AbstractFilter? SelectedFilter
        {
            get { return selectedFilter; }
            set { SetProperty(ref selectedFilter, value); }
        }

        public FilterViewModel(VideoDeviceViewModel videoDevice)
        {
            VideoDevice = videoDevice;
        }

        public void CaptureImage(ResultModel result)
        {
            if (FilterSets == null)
            {
                FilterSets = result.Detector.FilterSets;
                SelectedFilterSet = FilterSets?.LastOrDefault();
                SelectedFilter = SelectedFilterSet?.Filters.LastOrDefault();
            }
        }
    }
}
