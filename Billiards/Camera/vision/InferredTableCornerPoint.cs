using System.Drawing;
using System.Linq;
using Billiard.Camera.vision.algorithms;
using Billiard.Camera.vision.Geometries;

namespace Billiard.Camera.vision
{
    public class InferredTableCornerPoint 
    {

        public float Y { get; set; }
        public float X { get; set; }

        private int shortTimePointsSize;
        private int counter = 0;
        private CappedQueue<PointF> shortTimePoints;
        private CappedQueue<PointF> longTimePoints;

        public InferredTableCornerPoint() : this(75, 180)
        {
/*            this(ConfigurationProperties.readInt('Table_short_time_corner_points_size'),
                ConfigurationProperties.readInt('Table_short_time_corner_points_size'));
*/
        }

        public InferredTableCornerPoint(int shortTimePointsSize, int longTimePointsSize)
        {
            this.shortTimePointsSize = shortTimePointsSize;
            shortTimePoints = new CappedQueue<PointF>(shortTimePointsSize);
            longTimePoints = new CappedQueue<PointF>(longTimePointsSize);
        }

        public void recordPoint(PointF newPoint)
        {
            shortTimePoints.push(newPoint);
            PointF meanPoint = getMeanPoint();
            if (counter++ % shortTimePointsSize == 0)
                longTimePoints.push(meanPoint);

            PointF geometricMedianPoint = GeometricMath.getGeometricMedian(longTimePoints.elements);
            X = geometricMedianPoint.X;
            Y = geometricMedianPoint.Y;
        }

        public PointF getMeanPoint()
        {
            float size = shortTimePoints.elements.size();
            if (size == 0)
                return new PointF(0, 0);
            float meanX = shortTimePoints.elements.Sum(e => e.X) / size;
            float meanY = shortTimePoints.elements.Sum(e => e.Y) / size;
            return new PointF(meanX, meanY);
        }

        public PointF AsPoint()
        {
            return new PointF(X, Y);
        }
    }
}
