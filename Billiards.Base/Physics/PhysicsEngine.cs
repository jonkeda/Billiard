using System.Collections.ObjectModel;
using System.Numerics;
using Billiards.Base.Extensions;
using Billiards.Base.Filters;
using Billiards.Base.FilterSets;
using Billiards.Base.Physics.Prefabs;
using Billiards.Base.Utilities;
using OpenCvSharp;

namespace Billiards.Base.Physics
{
    public class PhysicsEngine
    {
        private float Length { get { return table.Length; } }
        private float Width { get { return table.Width; } }

        private readonly PTable table;
        private readonly List<PBall> balls = new();

        private const float StepSize = 0.001f;
        private const float BallRestitution = 0.98f;
        private const float StaticRestitution = 0.7f;
        private const float Ecs = 0.01f; // Error correction scalar

        public PhysicsEngine()
        {
            table = new CaramboleTable();
            Init();
        }

        #region Table initialisation

        public void Init()
        {
            float r = table.BallRadius;
            float y = Width / 2;

            // white
            AddBall(0, r, Length / 4, y + r * 10, BallColor.White);

            // red
            AddBall(7, r, (Length / 4) * 3, y, BallColor.Red);

            // yellow
            AddBall(9, r, Length / 4, y, BallColor.Yellow);
        }

        private void AddBall(int index, float r, float x, float y, BallColor ballColor)
        {
            var ball = new PBall(index, r, new Vector2(x, y), new Vector2(0, 0), ballColor);
            balls.Add(ball);
        }

        #endregion

        #region Physic Simulation

        public bool CalculateSolutions(ResultModel result)
        {
            if (result.Balls.Count < 3
                || result.Balls.Any(b => !b.TableRelativePosition.HasValue))
            {
                return false;
            }

            foreach (var ball in result.Balls)
            {
                if (ball.TableRelativePosition.HasValue)
                {
                    PBall? pBall = this.balls.Find(b => b.BallColor == ball.Color);
                    if (pBall != null)
                    {
                        pBall.position = ToAbsolutePoint(ball.TableRelativePosition.Value);
                    }
                }
            }
            result.Problems = CalculateSolutions(result.OnMainBall, result.Power);
            return true;
        }

        public ProblemCollection CalculateSolutions(bool onMainBall, float power)
        {
            PBall white = GetWhiteBall();
            PBall yellow = GetYellowBall();
            PBall red = GetRedBall();

            ProblemCollection problems;

            if (onMainBall)
            {
                problems = new ProblemCollection
                {
                    new(white, red, power, this),
                    new(white, yellow, power, this)
                };
            }
            else
            {
                problems = new ProblemCollection
                {
                    new(yellow, red, power, this),
                    new( yellow, white,  power, this)
                };
            }

/*            foreach (Problem problem in problems)
            {
                problem.Run();
            }
*/
                            Parallel.ForEach(problems, problem =>
                          {
                              try
                              {
                                  problem.Run();
                              }
                              catch (Exception e)
                              {
                                  string a = e.Message;
                              }
                          });
            return problems;

        }

        private PBall? Calculate(Vector2 force, bool animate, bool filter)
        {
            PBall[] ballsClones = new PBall[balls.Count];
            for (int i = 0; i < balls.Count; i++)
            {
                ballsClones[i] = new PBall(balls[i]);
                ballsClones[i].ClearPositions();
            }

            return Calculate(force, animate, filter, ballsClones, table);
        }

        private static PBall? Calculate(Vector2 force, bool animate, bool filter, PBall[] ballsClones, PTable table)
        {
            PBall cueBall = ballsClones[0];
            cueBall.velocity += force;

            bool isResting = false;
            while (!isResting)
            {
                isResting = Simulate(StepSize, ballsClones);

                DetectCollisions(table, ballsClones);

                if (animate)
                {
                    SaveBallPositions(ballsClones);
                }

                if (filter)
                {
                    if (cueBall.Collisions.TwoDifferentBallsHit())
                    {
                        if (cueBall.Collisions.Count(b => b.Ball != null) > 2)
                        {
                            return null;
                        }

                        if (ballsClones[1].Collisions.Count(b => b.Ball != null) > 1)
                            return null;
                        if (ballsClones[2].Collisions.Count(b => b.Ball != null) > 1)
                            return null;
                        SaveBallCollisions(ballsClones);
                        return cueBall;
                    }
                }
            }
            SaveBallCollisions(ballsClones);
            if (filter)
            {
                return null;
            }

            return cueBall;
        }

        private static void SaveBallCollisions(PBall[] pBalls)
        {
            int ballCount = pBalls.Length;
            for (int i = 0; i < ballCount; i++)
            {
                pBalls[i].SaveCollision(null, CollisionType.End);
            }
        }

        public static void SaveBallPositions(PBall[] pBalls)
        {
            int ballCount = pBalls.Length;
            for (int i = 0; i < ballCount; i++)
            {
                pBalls[i].SavePosition();
            }
        }

        private static bool Simulate(float dt, IReadOnlyList<PBall> pBalls)
        {
            bool isResting = true;
            int ballCount = pBalls.Count;
            for (int i = 0; i < ballCount; i++)
            {
                isResting &= pBalls[i].Simulate(dt);

            }
            return isResting;
        }

        private static void DetectCollisions(PStaticObject staticObject, IReadOnlyList<PBall> pBalls)
        {
            int ballCount = pBalls.Count;
            for (int i = 0; i < ballCount; i++)
            {
                if (StaticCollisionDetection(staticObject, pBalls[i]))
                {
                    StaticCollisionResolution(staticObject, pBalls[i]);
                }
            }

            for (int i = 0; i < ballCount; i++)
            {
                for (int j = i + 1; j < ballCount; j++)
                {
                    if (DynamicCollisionDetection(pBalls[i], pBalls[j]))
                    {
                        DynamicCollisionResolution(pBalls[i], pBalls[j]);
                    }
                }
            }
        }

        private static readonly float BallPow = (float)Math.Pow(PTable.BallRadiusConst + PTable.BallRadiusConst, 2);

        private static bool DynamicCollisionDetection(PBall ball1, PBall ball2)
        {
            return (ball1.position - ball2.position).LengthSquared() < BallPow;
        }

        private static void DynamicCollisionResolution(PBall ball1, PBall ball2)
        {
            float penetrationDepth = (ball1.position - ball2.position).Length() - ball1.r - ball2.r;

            if (!ball1.velocity.IsZero())
            {
                ball1.position += (Vector2.Normalize(ball1.velocity) * penetrationDepth);
            }

            if (!ball2.velocity.IsZero())
            {
                ball2.position += (Vector2.Normalize(ball2.velocity) * penetrationDepth);
            }

            Vector2 normal = Vector2.Normalize(ball2.position - ball1.position);

            Vector2 normalVelocity1 = normal * Vector2.Dot(normal, ball1.velocity);
            Vector2 tangentialVelocity1 = ball1.velocity - normalVelocity1;

            Vector2 normalVelocity2 = -normal * Vector2.Dot(-normal, ball2.velocity);
            Vector2 tangentialVelocity2 = ball2.velocity - normalVelocity2;

            ball1.velocity = normalVelocity2 * BallRestitution + tangentialVelocity1;
            ball2.velocity = normalVelocity1 * BallRestitution + tangentialVelocity2;

            ball1.position += normal * ((penetrationDepth - Ecs) * 0.5f);
            ball2.position -= normal * ((penetrationDepth - Ecs) * 0.5f);

            ball1.SaveCollision(ball2, CollisionType.Ball);
            ball2.SaveCollision(ball1, CollisionType.Ball);
        }

        private static bool StaticCollisionDetection(PStaticObject staticObject, PBall ball)
        {
            return staticObject.Collides(ball.position, ball.r);
        }

        private static void StaticCollisionResolution(PStaticObject staticObject, PBall ball)
        {
            float penetrationDepth = staticObject.MinDistance(ball.position, ball.r) - ball.r;

            Vector2 normal = -SDFOp.GetNormal(ball.position, staticObject.MinDistance);

            Vector2 normalVelocity = normal * Vector2.Dot(normal, ball.velocity);
            Vector2 tangentialVelocity = ball.velocity - normalVelocity;

            ball.velocity = -normalVelocity * StaticRestitution + tangentialVelocity;

            ball.position += normal * (penetrationDepth - Ecs);

            ball.SaveCollision(null, CollisionType.Cushion);
        }
        #endregion

        public PBall? GetWhiteBall()
        {
            return balls?.Find(b => b.BallColor == BallColor.White);
        }

        public PBall? GetYellowBall()
        {
            return balls?.Find(b => b.BallColor == BallColor.Yellow);
        }

        public PBall? GetRedBall()
        {
            return balls?.Find(b => b.BallColor == BallColor.Red);
        }
        
        private SolutionCollection CalculateSolutionsDirectOnBall(PBall cue, PBall other, float power)
        {
            Vector2 line = Vector2.Normalize(cue.position - other.position);

            Vector2 posA = (line.PerpendicularA() * cue.r * 2) + other.position;
            Vector2 posB = (line.PerpendicularB() * cue.r * 2) + other.position;

            Vector2 from = Vector2.Normalize(posA - cue.position);

            float fromAngle = from.GetAngle();

            Vector2 to = Vector2.Normalize(posB - cue.position);

            float toAngle = to.GetAngle();

            SolutionCollection solutions = new SolutionCollection();
            for (float angle = fromAngle; angle < toAngle; angle += 0.04f)
            {
                Vector2 n = MathV.GetVector(angle) * power;
                PBall? cueClone = Calculate(n, false, true);
                if (cueClone != null
                    && AllowedSolution(cueClone))
                {
                    solutions.Add(new Solution(cueClone.Collisions));
                }
            }

            return solutions;
        }

        private bool AllowedSolution(PBall cueBall)
        {
            return cueBall.Collisions.Count < 2 + 2 + 3;
        }

        public class ProblemCollection : Collection<Problem>
        {

        }

        public class Problem
        {
            public BallColor Color { get; set; }
            private readonly PBall cue;
            private readonly PBall other;
            private readonly float power;
            private readonly PhysicsEngine physicsEngine;
            public SolutionCollection? Solutions { get; private set; }

            public Problem(PBall cue, PBall other, float power, PhysicsEngine physicsEngine)
            {
                this.Color = other.BallColor;
                this.cue = cue;
                this.other = other;
                this.power = power;
                this.physicsEngine = physicsEngine;
            }


            public void Run()
            {
                Solutions = physicsEngine.CalculateSolutionsDirectOnBall(cue, other, power);

            }
        }

        private Vector2 ToAbsolutePoint(Point2f p)
        {
            return new Vector2(p.X * Length, p.Y * Width);
        }


        private Vector2 FromRelative(Mat frame, Point2f p)
        {
            if (frame.Height > frame.Width)
            {
                return new Vector2(p.Y * Length / frame.Height, Width - (p.X * Width / frame.Width));
            }
            return new Vector2(p.X * Length / frame.Width, p.Y * Width / frame.Height);
        }
    }
}