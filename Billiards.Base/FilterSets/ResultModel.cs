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

        public bool OnMainBall { get; set; } = true;

        public float Power { get; set; } = 1500;
        public PhysicsEngine.ProblemCollection? Problems { get; set; }
    }
}
