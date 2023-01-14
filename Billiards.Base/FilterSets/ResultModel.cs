using Billiards.Base.Filters;
using Billiards.Base.Physics;
using OpenCvSharp;

namespace Billiards.Base.FilterSets
{
    public class ResultModel
    {
        public ResultModel()
        {
            Now = DateTime.Now;
        }

        public Mat? Image { get; set; }

        public List<Point2f>? Corners { get; set; }

        public ResultBallCollection Balls { get; set; } = new();

        public CaramboleDetector? Detector { get; set; }
        public DateTime Now { get; }

        public BallColor CueBallColor { get; set; } = BallColor.White;

        public float Power { get; set; } = 1500;
        public PhysicsEngine.ProblemCollection? Problems { get; set; }
        public bool Found { get; set; }
    }
}
