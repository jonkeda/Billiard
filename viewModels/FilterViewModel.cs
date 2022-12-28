using System.Diagnostics.Metrics;
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

            //FilterSets.Add(new FindHsvContoursFilterSet());
            //FilterSets.Add(new HsvChannelsFilterSet());
            var table = FilterSets.AddSet(new TableDetectorSet());
            FilterSets.AddSet(new BallDetectorSet(table.ResultFilter()));

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

    public class TableDetectorSet : FilterSet
    {
        public TableDetectorSet() : base("Find table")
        {
            var original = Original();
            var hsv = CvtColorBgr2Hsv();
            ExtractChannel(hsv, 0);
            GaussianBlur();
            MorphClose();
            MorphOpen();
            var flood = FloodFill(255);
            Mask();
            DrawBoundingRect().BoundingRect = flood;
            var corners = FindCorners();
            corners.BoundingRect = flood;
            WarpPerspective(original).PointsFilter = corners;
        }

        public AbstractFilter ResultFilter()
        {
            return Filters.LastOrDefault();
        }
    }

    public class BallDetectorSet : FilterSet
    {
        public BallDetectorSet(AbstractFilter filter) : base("Find balls")
        {
            var original = Clone(filter);
            var hsv = CvtColorBgr2Hsv();
            ExtractChannel(hsv, 0);
            GaussianBlur();
            FloodFill(255);
            Mask();
            MorphClose();
            var morph = MorphOpen();
            Filters.AddFilter(new FloodFillCornersFilter(morph, 255));
            Not();
            var masked = ToMask();
            var canny = Canny();

            And(hsv).MaskFilter = masked;
            Histogram().MaskFilter = masked;

            And(original).MaskFilter = masked;
            Histogram().MaskFilter = masked;
            /*
            DrawBoundingRect().BoundingRect = flood;
            var corners = FindCorners();
            corners.BoundingRect = flood;
            WarpPerspective(original).PointsFilter = corners;
            */
        }
    }

}
