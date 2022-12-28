using System.Linq;
using Billiard.Models;
using Billiard.UI;
using Emgu.CV;

namespace Billiard.viewModels
{
    public class FilterViewModel : ViewModel
    {
        public FilterViewModel(VideoDeviceViewModel videoDevice)
        {
            VideoDevice = videoDevice;
            videoDevice.CaptureImage += VideoDevice_CaptureImage;
            InitFilters();
        }

        private AbstractFilter selectedFilter;
        private FilterSet selectedFilterSet;
        private FilterSetCollection filterSets = new();
        public VideoDeviceViewModel VideoDevice { get; }

        public FilterSet SelectedFilterSet
        {
            get { return selectedFilterSet; }
            set { SetProperty(ref selectedFilterSet, value); }
        }

        public FilterSetCollection FilterSets
        {
            get { return filterSets; }
            set { SetProperty(ref filterSets, value); }
        }

        public AbstractFilter SelectedFilter
        {
            get { return selectedFilter; }
            set { SetProperty(ref selectedFilter, value); }
        }

        private void VideoDevice_CaptureImage(object sender, CaptureEvent e)
        {
            Mat image = e.Image;
            if (image == null)
            {
                return;
            }

            FilterSets.ApplyFilters(image);
        }

        protected void InitFilters()
        {
            FilterSets.Clear();

            FilterSets.Add(new FindHsvContoursFilterSet());
            FilterSets.Add(new HsvChannelsFilterSet());
            FilterSets.Add(new FloodFillFilterSet());

            SelectedFilterSet = FilterSets.LastOrDefault();
            SelectedFilter = SelectedFilterSet?.Filters.LastOrDefault();
        }
    }

    public class FindHsvContoursFilterSet : FilterSet
    {
        public FindHsvContoursFilterSet() : base("Find HSV contours")
        {
            Original();
            CvtColorBgr2Hsv();
            GaussianBlur();
            Canny();
        }
    }

    public class HsvChannelsFilterSet : FilterSet
    {
        public HsvChannelsFilterSet() : base("HSV Channels")
        {
            Original();
            var hsv = CvtColorBgr2Hsv();
            ExtractChannel(hsv, 0);
            ExtractChannel(hsv, 1);
            ExtractChannel(hsv, 2);
        }
    }

    public class FloodFillFilterSet : FilterSet
    {
        public FloodFillFilterSet() : base("Flood fill")
        {
            var original = Original();
            var hsv = CvtColorBgr2Hsv();
            ExtractChannel(hsv, 0);
            GaussianBlur();
            MorphOpen();
            MorphOpen();
            var flood = FloodFill(255);
            Mask();
            DrawBoundingRect().BoundingRect = flood;
            var corners = FindCorners();
            corners.BoundingRect = flood;
            WarpPerspective(original).PointsFilter = corners;
        }
    }

}
