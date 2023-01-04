using System.Collections.ObjectModel;
using Billiards.Base.Filters;
using Billiards.Base.Physics;
using OpenCvSharp;

namespace Billiards.Base.FilterSets
{
    public class ResultBallCollection : Collection<ResultBall>
    {

    }

    public class ResultBall
    {
        public ResultBall(BallColor color, 
            Point2f? tableRelativePosition,
            Point2f? imageRelativePosition)
        {
            Color = color;
            TableRelativePosition = tableRelativePosition;
            ImageRelativePosition = imageRelativePosition;
        }

        public BallColor Color { get; set; }
        public Point2f? TableRelativePosition { get; set; }
        public Point2f? ImageRelativePosition { get; set; }
        public Point2f? ImageAbsolutePoint { get; set; }
    }

    public class ResultModel
    {
        public Mat? Image { get; set; }

        public List<Point2f>? Corners { get; set; }

        public ResultBallCollection Balls { get; set; } = new();

        public CaramboleDetector? Detector { get; set; }
        public DateTime Now { get; set; }

        public bool OnMainBall { get; set; } = true;

        public float Power { get; set; } = 1500;
        public PhysicsEngine.ProblemCollection? Problems { get; set; }
    }
}
