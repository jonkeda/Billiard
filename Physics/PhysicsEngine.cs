using Physics.Prefabs;
using Physics.Triggers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using Billiard.Physics;
using Utilities;
using System.Windows.Media.Imaging;
using Billiard.Utilities;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using FlowDirection = System.Windows.FlowDirection;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;

namespace Physics
{
    class PhysicsEngine
    {
        public const float stepSize = 0.001f;

        public float Length
        {
            get { return table.Length; }
        }

        public float LengthD
        {
            get { return table.Length; }
        }

        public float Width { get { return table.Width; } }
        public float WidthD { get { return table.Width; } }


        public readonly GameType gameType;

        public Force Force { get; } = new Force();

        public readonly PTable table;
        public readonly List<PBall> balls = new List<PBall>();
        public readonly PTrigger trigger;

        public readonly PJar jar = new PJar();

        private const float ballRestitution = 0.98f;
        private const float staticRestitution = 0.7f;
        private const float ecs = 0.01f; // Error correction scalar

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

            float r = table.BallRadius;
            float x = 697;
            float y = Width / 2;

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
                PBall ball = AddBall(0, r, Length / 4, y + r * 10, Brushes.White);

                // red
                AddBall(7, r, (Length / 4) * 3, y, Brushes.Red);

                // yellow
                AddBall(9, r, Length / 4, y, Brushes.Yellow);

                ball.DrawTrajectory = true;
            }
        }

        private void ClearBallLists()
        {
            balls.Clear();
        }

        private PBall AddBall(int index, float r, float x, float y, Brush color)
        {
            var ball = new PBall(index, r, new Vector2(x, y), new Vector2(0, 0), color: color);
            balls.Add(ball);
            return ball;
        }

        #endregion

        #region Physic Simulation

        public PBall Calculate(Vector2 force, bool animate, bool filter)
        {
            PBall[] ballsClones = new PBall[balls.Count];
            for (int i = 0; i < balls.Count; i++)
            {
                ballsClones[i] = new PBall(balls[i]);
                balls[i].ClearPositions();
            }
            PBall cueBall = ballsClones[0];
            cueBall.velocity += force;

            bool isResting = false;
            while (!isResting)
            {
                isResting = Simulate(stepSize, ballsClones, animate);

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

        private void SaveBallCollisions(PBall[] pBalls)
        {
            int ballCount = pBalls.Length;
            for (int i = 0; i < ballCount; i++)
            {
                pBalls[i].SaveCollision(null, CollisionType.End);
            }
        }

        public void Step()
        {
            float dt = stepSize;

            if (Resting) return;

            Resting = Simulate(dt, balls.ToArray(), true);

            DetectCollisions(table, balls.ToArray());

            CheckTriggers();
        }

        public static void SaveBallPositions(PBall[] pBalls)
        {
            int ballCount = pBalls.Length;
            for (int i = 0; i < ballCount; i++)
            {
                pBalls[i].SavePosition();
            }
        }

        private bool Simulate(float dt, PBall[] pBalls, bool animate)
        {
            bool isResting = true;
            int ballCount = pBalls.Length;
            for (int i = 0; i < ballCount; i++)
            {
                isResting &= pBalls[i].Simulate(dt, animate);

            }
            return isResting;
        }

        public void ApplyForce(PBall ball, Vector2 force)
        {
            ball.velocity += force;
            Resting = false;
        }

        private static void DetectCollisions(PStaticObject staticObject, PBall[] pBalls)
        {
            int ballCount = pBalls.Length;
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

        private static readonly float ballPow = (float)Math.Pow(PTable.BallRadiusConst + PTable.BallRadiusConst, 2);

        public static bool DynamicCollisionDetection(PBall ball1, PBall ball2)
        {
            return (ball1.position - ball2.position).LengthSquared() < ballPow;
        }

        private static void DynamicCollisionResolution(PBall ball1, PBall ball2)
        {
            float penetrationDepth = (ball1.position - ball2.position).Length() - ball1.r - ball2.r;

            if (!ball1.velocity.Zero())
            {
                ball1.position += (ball1.velocity.Normalize() * penetrationDepth);
            }

            if (!ball2.velocity.Zero())
            {
                ball2.position += (ball2.velocity.Normalize() * penetrationDepth);
            }

            Vector2 normal = (ball2.position - ball1.position).Normalize();

            Vector2 normalVelocity1 = normal * Vector2.Dot(normal, ball1.velocity);
            Vector2 tangentialVelocity1 = ball1.velocity - normalVelocity1;

            Vector2 normalVelocity2 = -normal * Vector2.Dot(-normal, ball2.velocity);
            Vector2 tangentialVelocity2 = ball2.velocity - normalVelocity2;

            ball1.velocity = normalVelocity2 * ballRestitution + tangentialVelocity1;
            ball2.velocity = normalVelocity1 * ballRestitution + tangentialVelocity2;

            ball1.position += normal * ((penetrationDepth - ecs) * 0.5f);
            ball2.position -= normal * ((penetrationDepth - ecs) * 0.5f);

            ball1.SaveCollision(ball2, CollisionType.Ball);
            ball2.SaveCollision(ball1, CollisionType.Ball);
        }

        public static bool StaticCollisionDetection(PStaticObject staticObject, PBall ball)
        {
            return staticObject.Collides(ball.position, ball.r);
        }

        private static void StaticCollisionResolution(PStaticObject staticObject, PBall ball)
        {
            float penetrationDepth = staticObject.MinDistance(ball.position, ball.r) - ball.r;

            Vector2 normal = -SDFOp.GetNormal(ball.position, staticObject.MinDistance);

            Vector2 normalVelocity = normal * Vector2.Dot(normal, ball.velocity);
            Vector2 tangentialVelocity = ball.velocity - normalVelocity;

            ball.velocity = -normalVelocity * staticRestitution + tangentialVelocity;

            ball.position += normal * (penetrationDepth - ecs);

            ball.SaveCollision(null, CollisionType.Cushion);
        }
        #endregion

        #region Distance Helpers
        private float SceneSDF(Vector2 p, float r = 0, bool ignoreLodGroups = true)
        {
            return Math.Min(GetBallsMinDistance(p), table.MinDistance(p, r, ignoreLodGroups));
        }

        private float GetBallsMinDistance(Vector2 p)
        {
            float distance = float.PositiveInfinity;

            foreach (PBall ball in balls)
            {
                if (ball.Equals(balls[^1])) continue;
                distance = Math.Min(distance, (p - ball.position).Length() - ball.r);
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

        public Force CalculateForce(Vector2 ballPosition)
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

                FormattedText formattedText = new(
                    (DateTime.Now - now).TotalMilliseconds.ToString(),
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    32,
                    Brushes.AntiqueWhite, 1.25);

                drawingContext.DrawText(formattedText, new Point(0, 0));

                drawingContext.Close();
            }
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)Length, (int)Width, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            return bitmap;
            //return new DrawingImage(visual.Drawing);  
        }

        private SolutionCollection CalculateSolutionsDirectOnBall(PBall cue, PBall other, Brush brush, float power)
        {
            Vector2 line = (cue.position - other.position).Normalize();

            Vector2 posA = (line.PerpendicularA() * cue.r * 2) + other.position;
            Vector2 posB = (line.PerpendicularB() * cue.r * 2) + other.position;

            /*            drawingContext.DrawLine(forceColor, cue.position, other.position);
                        drawingContext.DrawEllipse(null, forceColor, cue.position, cue.r, cue.r);
                        drawingContext.DrawEllipse(null, forceColor, other.position, cue.r, cue.r);

                        drawingContext.DrawEllipse(null, forceColor, posA, cue.r, cue.r);
                        drawingContext.DrawEllipse(null, forceColor, posB, cue.r, cue.r);

                        drawingContext.DrawLine(forceColor, cue.position, posA);
                        drawingContext.DrawLine(forceColor, cue.position, posB);
            */
            Vector2 from = (posA - cue.position).Normalize();

            float fromAngle = from.GetAngle();

            Vector2 to = (posB - cue.position).Normalize();

            float toAngle = to.GetAngle();

            //float angleDif = toAngle - fromAngle;
            SolutionCollection solutions = new SolutionCollection();
            for (float angle = fromAngle; angle < toAngle; angle += 0.04f)
            {
                Vector2 n = MathV.GetVector(angle) * power;
                PBall cueClone = Calculate(n, false, true);
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
            private readonly float power;
            private readonly PhysicsEngine physicsEngine;
            public Pen Color { get; }
            public SolutionCollection Solutions { get; private set; }


            public Problem(PBall cue, PBall other, Brush brush, float power, PhysicsEngine physicsEngine)
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