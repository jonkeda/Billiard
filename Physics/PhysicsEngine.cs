using Physics.Prefabs;
using Physics.Triggers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Billiard.Physics;
using Utilities;
using System.Windows.Media.Imaging;
using FlowDirection = System.Windows.FlowDirection;

namespace Physics
{
    class PhysicsEngine
    {
        public const double stepSize = 0.001;

        public double Length
        {
            get { return table.Length; }
        }

        public double Width { get { return table.Width;} }


        public readonly GameType gameType;

        public  Force Force { get; } = new Force();

        public readonly PTable table;
        public readonly List<PBall> balls = new List<PBall>();
        public readonly PTrigger trigger;

        public readonly PJar jar = new PJar();

        private const double ballRestitution = 0.98;
        private const double staticRestitution = 0.7;
        private const double ecs = 0.01; // Error correction scalar

        public PhysicsEngine(GameType gameType)
        {
            if (gameType == GameType.Billiart)
            {
                table = new BilliartTable();
            }
            else
            {
                table = new PoolTable();
            }
            trigger = new PTrigger(table.Length, table.Width);
            this.gameType = gameType;
            Init();
        } 

        public bool Resting { get; private set; } = false;

        #region Table initialisation
        public void Init()
        {
            ClearBallLists();

            double r = table.BallRadius;
            double x = 697;
            double y = Width / 2;

            if (gameType == GameType.Pool)
            {
                AddBall(12, r, x + 14 * 4, y - 32, Brushes.Yellow);
                AddBall(6, r, x + 14 * 4, y - 16, Brushes.Yellow);
                AddBall(15, r, x + 14 * 4, y, Brushes.Yellow);
                AddBall(13, r, x + 14 * 4, y + 16, Brushes.Yellow);
                AddBall(5, r, x + 14 * 4, y + 32, Brushes.Yellow);

                AddBall(4, r, x + 14 * 3, y - 24, Brushes.Yellow);
                AddBall(14, r, x + 14 * 3, y - 8, Brushes.Yellow);
                AddBall(7, r, x + 14 * 3, y + 8, Brushes.Yellow);
                AddBall(11, r, x + 14 * 3, y + 24, Brushes.Yellow);

                AddBall(3, r, x + 14 * 2, y - 16, Brushes.Yellow);
                AddBall(8, r, x + 14 * 2, y, Brushes.Yellow);
                AddBall(10, r, x + 14 * 2, y + 16, Brushes.Yellow);

                AddBall(2, r, x + 14, y - 8, Brushes.Yellow);
                AddBall(9, r, x + 14, y + 8, Brushes.Yellow);

                AddBall(1, r, x, y, Brushes.Yellow);

                AddBall(0, r, Width / 2, y, Brushes.Yellow);

            }
            else
            {
                // white
                PBall ball = AddBall(0, r, Length / 4, y, Brushes.White);

                // red
                AddBall(7, r, (Length / 4) * 3, y, Brushes.Red);

                // yellow
                AddBall(9, r, Length / 4, y + r * 10, Brushes.Yellow);

                ball.DrawTrajectory = true;
            }
        }

        private void ClearBallLists()
        {
            balls.Clear();
        }

        private PBall AddBall(int index, double r, double x, double y, Brush color )
        {
            var ball = new PBall(index, r, new Vector2D(x, y), new Vector2D(0,0), color: color);
            balls.Add(ball);
            return ball;
        }

        #endregion

        #region Physic Simulation

        public PBall Calculate(Vector2D force, bool animate)
        {
            List<PBall> ballsClones = new List<PBall>();
            foreach (PBall ball in balls)
            {
                ballsClones.Add(new PBall(ball));
                ball.ClearPositions();
            }

            PBall cueBall = ballsClones.Find(b => b.index == 0);

            cueBall.velocity += force;

            bool isResting = false;
            while (!isResting)
            {
                isResting = Simulate(stepSize, ballsClones, animate);

                DetectCollisions(table, ballsClones, true);

                if (animate)
                {
                    SaveBallPositions(ballsClones);
                }

                if (cueBall.Collisions.TwoDifferentBallsHit())
                {
                    if (cueBall.Collisions.Count(b => b.Ball != null) > 2)
                    {
                        return null;
                    }
                    if (GetYellowBall().Collisions.Count(b => b.Ball != null) > 1)
                        return null;
                    if (GetRedBall().Collisions.Count(b => b.Ball != null) > 1)
                        return null;
                    SaveBallCollisions();
                    return cueBall;
                }
            }
            SaveBallCollisions();

            return null;
        }

        private void SaveBallCollisions()
        {
            foreach (PBall ball in balls)
            {
                ball.SaveCollision(null, CollisionType.End);
            }
        }

        public void Step()
        {
            double dt = stepSize;

            if (Resting) return;

            Resting = Simulate(dt, balls, true);

            DetectCollisions(table, balls, true);

            CheckTriggers();
        }

        public static void SaveBallPositions(List<PBall> balls)
        {
            foreach (PBall ball in balls)
            {
                ball.SavePosition();
            }
        }

        private bool Simulate(double dt, List<PBall> pBalls, bool animate)
        {
            bool isResting = true;
            foreach (PBall ball in pBalls)
            {
                ball.Simulate(dt, animate);
                if (!ball.Resting) isResting = false;
            }

            return isResting;
        }

        public void ApplyForce(PBall ball, Vector2D force)
        {
            ball.velocity += force;
            Resting = false;
        }

        private void ClearBallPositions()
        {
            foreach (PBall ball in balls)
            {
                ball.ClearPositions();
            }
        }

        private static void DetectCollisions(PStaticObject staticObject, List<PBall> pBalls, bool recordCollisions)
        {
            //for (int runs = 0; runs < 64; runs++)
            //{
                //bool collision = false;

                foreach (PBall ball in pBalls)
                {
                    if (StaticCollisionDetection(staticObject, ball))
                    {
                        //collision = true;
                        StaticCollisionResolution(staticObject, ball, recordCollisions);
                    }
                }

                for (int i = 0; i < pBalls.Count; i++)
                {
                    for (int j = i + 1; j < pBalls.Count; j++)
                    {
                        if (DynamicCollisionDetection(pBalls[i], pBalls[j]))
                        {
                            //collision = true;
                            DynamicCollisionResolution(pBalls[i], pBalls[j], recordCollisions);
                        }
                    }
                }

/*                if (!collision) break;
            }*/
        }

        public static bool DynamicCollisionDetection(PBall ball1, PBall ball2)
        {
            return (ball1.position - ball2.position).SquaredLength - Math.Pow(ball1.r + ball2.r, 2) < 0;
        }

        private static void DynamicCollisionResolution(PBall ball1, PBall ball2, bool recordCollisions)
        {
            double penetrationDepth = (ball1.position - ball2.position).Length - ball1.r - ball2.r;

            if (!ball1.velocity.Zero)
            {
                ball1.position += (ball1.velocity.Normalize() * penetrationDepth);
            }

            if (!ball2.velocity.Zero)
            {
                ball2.position += (ball2.velocity.Normalize() * penetrationDepth);
            }

            Vector2D normal = (ball2.position - ball1.position).Normalize();

            Vector2D normalVelocity1 = normal * MathV.Dot(normal, ball1.velocity);
            Vector2D tangentialVelocity1 = ball1.velocity - normalVelocity1;

            Vector2D normalVelocity2 = -normal * MathV.Dot(-normal, ball2.velocity);
            Vector2D tangentialVelocity2 = ball2.velocity - normalVelocity2;

            ball1.velocity = normalVelocity2 * ballRestitution + tangentialVelocity1;
            ball2.velocity = normalVelocity1 * ballRestitution + tangentialVelocity2;

            ball1.position += normal * (penetrationDepth - ecs) * 0.5;
            ball2.position -= normal * (penetrationDepth - ecs) * 0.5;

            ball1.SaveCollision(ball2, CollisionType.Ball);
            ball2.SaveCollision(ball1, CollisionType.Ball);

            //if (recordCollisions) OnCollision(new CollisionEvent(ball1.position, Math.Max(normalVelocity1.Length, normalVelocity2.Length), 1));
        }

        public static bool StaticCollisionDetection(PStaticObject staticObject, PBall ball)
        {
            return staticObject.Collides(ball.position, ball.r);
            // return staticObject.MinDistance(ball.position, ball.r) - ball.r < 0;
        }

        private static void StaticCollisionResolution(PStaticObject staticObject, PBall ball, bool recordCollisions)
        {
            double penetrationDepth = staticObject.MinDistance(ball.position, ball.r) - ball.r;

            Vector2D normal = -SDFOp.GetNormal(ball.position, staticObject.MinDistance);

            Vector2D normalVelocity = normal * MathV.Dot(normal, ball.velocity);
            Vector2D tangentialVelocity = ball.velocity - normalVelocity;

            ball.velocity = -normalVelocity * staticRestitution + tangentialVelocity;

            ball.position += normal * (penetrationDepth - ecs);

            ball.SaveCollision(null, CollisionType.Cushion);

            //if (recordCollisions) OnCollision(new CollisionEvent(ball.position, normalVelocity.Length, 0));
        }
        #endregion

        #region Distance Helpers
        private double SceneSDF(Vector2D p, double r = 0, bool ignoreLodGroups = true)
        {
            return Math.Min(GetBallsMinDistance(p), table.MinDistance(p, r, ignoreLodGroups));
        }

        private double GetBallsMinDistance(Vector2D p)
        {
            double distance = double.PositiveInfinity;

            foreach(PBall ball in balls)
            {
                if (ball.Equals(balls[^1])) continue;
                distance = Math.Min(distance, (p - ball.position).Length - ball.r);
            }

            return distance;
        }
        #endregion

        #region Trigger Checks
        private void CheckTriggers()
        {
            for (int i = 0; i < balls.Count; i++)
            {
                PBall ball = balls[i];

                if (trigger.CheckTrigger(ball))
                {
                    TriggerEvent triggerEvent = new TriggerEvent
                    {
                        ball = ball
                    };

                    OnTrigger(triggerEvent);
                }
            }
        }
        #endregion

        #region Trajectory
        public Trajectory CalculateTrajectory(Vector2D rayOrigin, Vector2D rayDirection, double radius, double errorTolerance = 0.5)
        {
            double ballsDistance;
            double tableDistance;
            Vector2D origin = rayOrigin;

            for (int steps = 0; steps < 64; steps++)
            {
                ballsDistance = GetBallsMinDistance(rayOrigin) - radius;

                tableDistance = table.MinDistance(rayOrigin, 0, true) - radius;

                if (ballsDistance < errorTolerance)
                {
                    return new Trajectory(origin, rayOrigin, -SDFOp.GetNormal(rayOrigin, SceneSDF));
                }

                if (tableDistance < errorTolerance) break;

                rayOrigin += rayDirection * Math.Min(ballsDistance, tableDistance);
            }

            Vector2D normal = SDFOp.GetNormal(rayOrigin, SceneSDF);
            Vector2D normalV = normal * MathV.Dot(normal, rayDirection);

            return new Trajectory(origin, rayOrigin, (-normalV * staticRestitution + rayDirection - normalV).Normalize());
        }
        #endregion

        #region Events
        public event EventHandler<TriggerEvent> Trigger;
        // public event EventHandler<CollisionEvent> Collision;
        protected virtual void OnTrigger(TriggerEvent e)
        {
            Trigger?.Invoke(this, e);
        }

/*        protected virtual void OnCollision(CollisionEvent e)
        {
            Collision?.Invoke(this, e);
        }*/

        #endregion

        public Force CalculateForce(Vector2D ballPosition)
        {
            Force.Position = ballPosition;
            return Force;
        }

        public PBall GetCueBall()
        {
            return balls.Find(b => b.index == 0);
        }

        public PBall GetYellowBall()
        {
            return balls.Find(b => b.index == 9);
        }

        public PBall GetRedBall()
        {
            return balls.Find(b => b.index == 7);
        }

        public RenderTargetBitmap CalculateSolutions()
        {
            DateTime now = DateTime.Now;
            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                drawingContext.PushClip(new RectangleGeometry(new Rect(new Point(0, 0), new Point(Length, Width))));
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), null, new Rect(0, 0, Length, Width));

                PBall cue = GetCueBall();
                PBall yellow = GetYellowBall();
                PBall red = GetRedBall();

/*                SolutionCollection solutionsRed = CalculateSolutionsDirectOnBall(cue, red, Brushes.Orange, 1500);

                SolutionCollection solutionsYellow = CalculateSolutionsDirectOnBall(cue, yellow, Brushes.GreenYellow, 1500);
*/
                /*                CalculateSolutionsDirectOnBall(cue, red, drawingContext, Brushes.Orange, 1250);

                                CalculateSolutionsDirectOnBall(cue, yellow, drawingContext, Brushes.GreenYellow, 1250);
                */
                ProblemCollection problems = new ProblemCollection
                {
                    new (cue, red, Brushes.Orange, 1500, this),
                    new (cue, yellow, Brushes.GreenYellow, 1500, this)
                };

                foreach (Problem problem in problems)
                {
                    problem.Run();
                }
/*                Parallel.ForEach(problems, problem =>
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
*/
                foreach (Problem problem in problems)
                {
                    foreach (Solution solution in problem.Solutions)
                    {
                        drawingContext.DrawGeometry(null, problem.Color, solution.Collections.AsGeometry());
                    }
                }

                FormattedText formattedText = new (
                    (DateTime.Now - now).TotalMilliseconds.ToString(),
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    32,
                    Brushes.AntiqueWhite, 1.25);

                drawingContext.DrawText(formattedText, new Point(0,0));

                drawingContext.Close();
            }
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)Length, (int)Width, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            return bitmap;
            //return new DrawingImage(visual.Drawing);  
        }

        private SolutionCollection CalculateSolutionsDirectOnBall(PBall cue, PBall other, Brush brush, double power)
        {
            Vector2D line = (cue.position - other.position).Normalize();

            Vector2D posA = (line.PerpendicularA() * cue.r * 2) + other.position;
            Vector2D posB = (line.PerpendicularB() * cue.r * 2) + other.position;

/*            drawingContext.DrawLine(forceColor, cue.position, other.position);
            drawingContext.DrawEllipse(null, forceColor, cue.position, cue.r, cue.r);
            drawingContext.DrawEllipse(null, forceColor, other.position, cue.r, cue.r);

            drawingContext.DrawEllipse(null, forceColor, posA, cue.r, cue.r);
            drawingContext.DrawEllipse(null, forceColor, posB, cue.r, cue.r);

            drawingContext.DrawLine(forceColor, cue.position, posA);
            drawingContext.DrawLine(forceColor, cue.position, posB);
*/
            Vector2D from = (posA - cue.position).Normalize();

            double fromAngle = from.GetAngle();

            Vector2D to = (posB - cue.position ).Normalize();

            double toAngle = to.GetAngle();

            //double angleDif = toAngle - fromAngle;
            SolutionCollection solutions = new SolutionCollection();
            for (double angle = fromAngle; angle < toAngle; angle += 0.02)
            {
                Vector2D n = MathV.GetVector(angle) * power;
                PBall cueClone = Calculate(n, false);
                if (cueClone != null)
                {
                    if (AllowedSolution(cue))
                    {
                        solutions.Add(new Solution(cueClone.Collisions));
                    }
                }
            }

            return solutions;
        }

        private bool AllowedSolution(PBall cue)
        {
            return cue.Collisions.Count < 2 + 2 + 3;
        }

        public class ProblemCollection : Collection<Problem>
        {

        }

        public class Problem
        {
            private readonly PBall cue;
            private readonly PBall other;
            private readonly Brush brush;
            private readonly double power;
            private readonly PhysicsEngine physicsEngine;
            public  Pen Color { get; }
            public SolutionCollection Solutions { get; private set; }


            public Problem(PBall cue, PBall other, Brush brush, double power, PhysicsEngine physicsEngine)
            {
                this.cue = cue;
                this.other = other;
                this.brush = brush;
                Color = new Pen(brush, 2)
                {
                    DashStyle = DashStyles.Solid
                };

                this.power = power;
                this.physicsEngine = physicsEngine;
            }

            public void Run()
            {
                Solutions = physicsEngine.CalculateSolutionsDirectOnBall(cue, other, brush, power);

            }
        }
    }

    class SolutionCollection : Collection<Solution>
    {
    }

    class Solution
    {
        public CollisionCollection Collections { get; }

        public Solution(CollisionCollection collections)
        {
            Collections = collections;
        }
    }
}