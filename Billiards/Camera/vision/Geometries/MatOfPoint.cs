using System.Drawing;
using Emgu.CV;

namespace Billiard.Camera.vision.Geometries
{
    public class MatOfPoint : IOutputArray
    {
        private PointF[] points;

        public MatOfPoint(params PointF[] points)
        {
            this.points = points;
        }

        public void Dispose()
        {
        }

        public InputArray GetInputArray()
        {
            return new InputArray((nint)points.Length, points);
        }

        public OutputArray GetOutputArray()
        {
            return new OutputArray((nint)points.Length, points);
        }
    }
}
