using System.Numerics;
using Billiards.Base.FilterSets;
using Billiards.Base.Physics;
using Billiards.Web.Shared;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;
using Collision = Billiards.Base.Physics.Collision;
using Point = Billiards.Web.Shared.Point;
using Solution = Billiards.Base.Physics.Solution;

namespace Billiards.Web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredictionController : ControllerBase
    {
        [HttpPost]
        public PredictionResponse Predict(PredictionRequest request)
        {
            PhysicsEngine physicsEngine = new PhysicsEngine();

            ResultBallCollection resultBalls = new ResultBallCollection();
            foreach (Ball ball in request.Balls)
            {
                resultBalls.Add(new ResultBall((Base.Filters.BallColor) ball.Color,
                    ConvertPoint(ball.TablePoint), null));
            }

            var result = new ResultModel
            {
                Balls = resultBalls,
                CueBallColor = (Base.Filters.BallColor)request.CueBall
            };
            if (!physicsEngine.CalculateSolutions(result))
            {
                return new PredictionResponse(new ProblemCollection());
            }

            ProblemCollection problems = new();
            if (result.Problems != null)
            {
                foreach (PhysicsEngine.Problem problem in result.Problems)
                {
                    Shared.SolutionCollection solutions = new();
                    if (problem.Solutions != null)
                    {
                        foreach (Solution solution in problem.Solutions)
                        {
                            Shared.CollisionCollection collisions = new();
                            foreach (Collision collision in solution.Collisions)
                            {
                                collisions.Add(new Shared.Collision(
                                    ConvertPoint(collision.Position), 
                                    (BallColor?)collision.Ball?.BallColor,
                                    (Shared.CollisionType)collision.CollisionType));
                                
                            }
                            solutions.Add(new Shared.Solution(collisions));
                        }
                    }
                    problems.Add(new Problem((BallColor)problem.Color, solutions));
                }
            }

            return new PredictionResponse(problems);
        }

        private static Point2f? ConvertPoint(Point? point)
        {
            if (point == null)
            {
                return null;
            }
            return new Point2f(point.X, point.Y);
        }

        private static Point ConvertPoint(Vector2 point)
        {
            return new Point(point.X, point.Y);
        }

        private static Point? ConvertPoint(Point2f? point)
        {
            if (!point.HasValue)
            {
                return null;
            }
            return new Point(point.Value.X, point.Value.Y);
        }

        private static BallColor ConvertColor(Billiards.Base.Filters.BallColor color)
        {
            return (BallColor) color;
        }



    }
}