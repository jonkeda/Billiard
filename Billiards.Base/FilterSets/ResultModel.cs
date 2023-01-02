using Billiard.Physics;
using OpenCvSharp;

namespace Billiards.Base.FilterSets
{
    public class ResultModel
    {
        public Mat Image { get; set; }

        public List<Point2f> Corners { get; set; }
        public Rect2f TableSize { get; set; }
        public Point2f? WhiteBallPoint { get; set; }
        public Point2f? YellowBallPoint { get; set; }
        public Point2f? RedBallPoint { get; set; }
        public CaramboleDetector Detector { get; set; }
        public DateTime Now { get; set; }

        public bool OnMainBall { get; set; }

        public float Power { get; set; } = 1500;
        public PhysicsEngine.ProblemCollection Problems { get; set; }
    }
}
