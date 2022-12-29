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
            var gaus = GaussianBlur();
            //var findPoint = Filters.AddFilter(new FindPointByColorFilter(gaus));
            //findPoint.Size = 20;
            //findPoint.Step = 10;

            MorphClose();
            MorphOpen();
            var flood = FloodFill(255);
            flood.MinimumArea = 14;
            flood.MaximumArea = 99;

            //flood.PointFilter = findPoint;
            var asMask = Mask();
            DrawBoundingRect().BoundingRect = flood;
            var corners = FindCorners();
            corners.BoundingRect = flood;
            //Filters.AddFilter(new FindCornerHarrisFilter(asMask));
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
            var gaus = GaussianBlur();
            // Filters.AddFilter(new FindPointByColorFilter(gaus));
            FloodFill(255);
            Mask();
            MorphClose();
            var morph = MorphOpen();
            Filters.AddFilter(new FloodFillCornersFilter(morph, 255));

            Not();
            var masked = ToMask();
            var canny = Canny();
            var contours = Contours();
            contours.MinimumArea = 500;
            contours.MaximumArea = 2000;
            contours.Resize = 0.7;

            And(hsv).MaskFilter = masked;
            Histogram().MaskFilter = masked;

            And(original).MaskFilter = masked;
            Histogram().MaskFilter = masked;

            var and1 = And(hsv);
            and1.ContourFilter = contours;
            and1.MaskContour = 0;
            var hist0 = Histogram();
            hist0.End = 100;

            var and2 = And(hsv);
            and2.ContourFilter = contours;
            and2.MaskContour = 1;
            var hist1 = Histogram();
            hist1.End = 100;

            var and3 = And(hsv);
            and3.ContourFilter = contours;
            and3.MaskContour = 2;
            var hist2 = Histogram();
            hist2.End = 100;

            var result = Filters.AddFilter(new BallResultFilter(original));
            result.ContourFilter = contours;
            result.Histogram0 = hist0;
            result.Histogram1 = hist1;
            result.Histogram2 = hist2;

        }
    }

}
