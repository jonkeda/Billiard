using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Documents;
using Billiard.Models;
using Billiard.UI;
using Emgu.CV;
using Point = System.Windows.Point;

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

        public BallResultFilter BallResultFilter { get; private set; }
        public IPointsFilter PointsFilter { get; set; }

        private void VideoDevice_CaptureImage(object sender, CaptureEvent e)
        {
            Mat image = e.Image;
            if (image == null)
            {
                return;
            }

            FilterSets.ApplyFilters(image);
        }

        public (List<Point> corners, Rect tableSize, 
            Point? whiteBallPoint, Point? yellowBallPoint, Point? redBallPoint) ApplyFilters(Mat image)
        {
            FilterSets.ApplyFilters(image);

            return (PointsFilter.Points,
                BallResultFilter.TableSize,
                BallResultFilter.WhiteBallPoint, 
                BallResultFilter.YellowBallPoint, 
                BallResultFilter.RedBallPoint);
        }

        protected void InitFilters()
        {
            FilterSets.Clear();

            //FilterSets.Add(new FindHsvContoursFilterSet());
            //FilterSets.Add(new HsvChannelsFilterSet());
            var table = FilterSets.AddSet(new TableDetectorSet());
            CornerDetectorSet corner = FilterSets.AddSet(new CornerDetectorSet(table.OriginalFilter, table.FloodFilter));
            var ball = FilterSets.AddSet(new BallDetectorSet(corner.ResultFilter()));


            BallResultFilter = ball.BallResultFilter;
            PointsFilter = corner.PointsFilter;

            SelectedFilterSet = FilterSets.LastOrDefault();
            SelectedFilter = SelectedFilterSet?.Filters.LastOrDefault();
        }
    }
}
