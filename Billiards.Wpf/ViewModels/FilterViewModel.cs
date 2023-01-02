using System.Collections.Generic;
using System.Linq;
using Billiard.UI;
using Billiards.Base.Filters;
using Billiards.Base.FilterSets;
using OpenCvSharp;

namespace Billiard.viewModels
{
    public class FilterViewModel : ViewModel
    {
        public FilterViewModel(VideoDeviceViewModel videoDevice)
        {
            VideoDevice = videoDevice;
            videoDevice.CaptureImage += VideoDeviceCaptureImage;
            InitFilters();

            Detector = new CaramboleDetector();

            filterSets = Detector.FilterSets;

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

        public CaramboleDetector Detector { get; private set; }


        public BallResultFilter BallResultFilter { get; private set; }
        public IPointsFilter PointsFilter { get; set; }

        private void VideoDeviceCaptureImage(object? sender, CaptureEvent e)
        {
            Mat image = e.Image;
            if (image == null)
            {
                return;
            }

            ApplyFilters(image);
        }

/*        public (List<Point> corners, Rect tableSize, 
            Point? whiteBallPoint, Point? yellowBallPoint, Point? redBallPoint) ApplyFilters(Mat image)
*/        public void ApplyFilters(Mat image)
        {
            FilterSets.ApplyFilters(image);

/*            return (PointsFilter.Points,
                BallResultFilter.TableSize,
                BallResultFilter.WhiteBallPoint, 
                BallResultFilter.YellowBallPoint, 
                BallResultFilter.RedBallPoint);
*/        }

        protected void InitFilters()
        {
            FilterSets.Clear();

            Detector = new CaramboleDetector();

            filterSets = Detector.FilterSets;

            BallResultFilter = Detector.BallResultFilter;
            PointsFilter = Detector.PointsFilter;

            SelectedFilterSet = FilterSets.LastOrDefault();
            SelectedFilter = SelectedFilterSet?.Filters.LastOrDefault();
        }
    }
    

}
