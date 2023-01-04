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
        public ResultBall(BallColor color, Point2f? location)
        {
            Color = color;
            Location = location;
        }

        public BallColor Color { get; set; }
        public Point2f? Location { get; set; }
    }

    public class ResultModel
    {
        public Mat? Image { get; set; }

        public List<Point2f>? Corners { get; set; }

        public ResultBallCollection Balls { get; set; } = new();

        public Point2f? WhiteBallPoint { get; set; }
        public Point2f? YellowBallPoint { get; set; }
        public Point2f? RedBallPoint { get; set; }

        public CaramboleDetector? Detector { get; set; }
        public DateTime Now { get; set; }

        public bool OnMainBall { get; set; } = true;

        public float Power { get; set; } = 1500;
        public PhysicsEngine.ProblemCollection? Problems { get; set; }
    }
}
