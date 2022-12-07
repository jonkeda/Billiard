using Physics;
using Physics.Triggers;
using Render;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Utilities;

namespace Billiard
{
    public partial class MainWindow 
    {
        private readonly PhysicsEngine physicsEngine;
        private readonly Renderer renderer;

        private long t = DateTime.Now.Ticks / 10000;
        private bool shot;


        #region Gameloop
        private void Update(object sender, EventArgs e)
        {
            if (calculating)
            {
                return;
            }
            UpdateUI();
            UpdatePhysics();
            UpdateRenderer();
        }

        private void UpdateUI()
        {

        }

        private void UpdatePhysics()
        {

            long nextT = DateTime.Now.Ticks / 10000;
            for (long i = t; i < nextT; i += 8)
            {
                physicsEngine.Step(0.008);
                if (nextT - i > 500) break;
            }

            t = DateTime.Now.Ticks / 10000;
        }

        private PBall GetCueBall()
        {
            return physicsEngine.GetCueBall();
        }

        private PBall GetYellowBall()
        {
            return physicsEngine.GetYellowBall();
        }

        private PBall GetRedBall()
        {
            return physicsEngine.GetRedBall();
        }

        private void UpdateRenderer()
        {
            renderer.Update();

            if (physicsEngine.Resting)
            {
                if (shot)
                {
                    CalculateSolutions();
                    shot = false;
                }
                Vector2D p = Mouse.GetPosition(Table);

                var (ballPosition, ballRadius) = GetCueBall();

                renderer.DrawQueue(ballPosition, ballRadius, p);

                CalculateForce(out var force);

                physicsEngine.Calculate(force, true);

                // (ballPosition - p).Normalize()
                renderer.DrawTrajectory(ballRadius, 
                    physicsEngine.CalculateTrajectory(ballPosition, physicsEngine.Force.VectorPower, ballRadius),
                    physicsEngine.CalculateForce(ballPosition));
            }
        }
        #endregion

        #region Gameeventhandlers and utitilies

        private void HitBall(object sender, RoutedEventArgs e)
        {
            Shoot();
        }

        private void Shoot()
        {
            if (!physicsEngine.Resting)
            {
                return;
            }

            var ball = CalculateForce(out var force);

            //soundManager.BreakSound(p, force.Length);

            physicsEngine.ApplyForce(ball, force);

            shot = true;
        }

        private PBall CalculateForce(out Vector2D force)
        {
            PBall ball = GetCueBall();

            Vector2D n = physicsEngine.Force.Vector;
            force = Math.Min(physicsEngine.Force.Power, 200) * 10 * n;

            
            return ball;
        }

/*        private void Trigger(object sender, TriggerEvent e)
        {
            physicsEngine.TransferBall(e.ball);
            renderer.RemoveBall(e.ball);
        }*/
        #endregion


        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            CompositionTarget.Rendering += Update;

            //soundManager = new SoundManager();

            physicsEngine = new PhysicsEngine(GameType.Billiart);
            //physicsEngine.Trigger += Trigger;
            //physicsEngine.Collision += soundManager.CollisionSound;

            renderer = new Renderer(Table, Queue, Overlay, Solutions, physicsEngine.Length, physicsEngine.Width);
            renderer.ResetAll(physicsEngine.balls);

            DataContext = physicsEngine;
        }

        private void UIElement_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            CalculateSolutions();
/*            var ball = CalculateForce(out var force);
*/
            // physicsEngine.Calculate(force);
        }


        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            double speed = 1;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                speed = 10;
            }
            else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                speed = 0.01;
            }
/*            else if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.LeftAlt))
            {
                speed = 10;
            }
*/            if (e.Key == Key.Up)
            {
                physicsEngine.Force.PowerUp();
            }
            else if (e.Key == Key.Down)
            {
                physicsEngine.Force.PowerDown();
            }
            else if (e.Key == Key.Left)
            {
                physicsEngine.Force.ClockWise(speed);
            }
            else if (e.Key == Key.Right)
            {
                physicsEngine.Force.CounterClockWise(speed);
            }
            else if (e.Key == Key.Space)
            {
                Shoot();
            }
            else if (e.Key == Key.Enter)
            {
                CalculateSolutions();
            }
            else if (e.Key == Key.D1)
            {
                MoveBallPosition(GetCueBall());
            }
            else if (e.Key == Key.D2)
            {
                MoveBallPosition(GetYellowBall());
            }
            else if (e.Key == Key.D3)
            {
                MoveBallPosition(GetRedBall());
            }

        }

        private void MoveBallPosition(PBall ball)
        {
            ball.position = Mouse.GetPosition(Table);
            CalculateSolutions();
        }

        private bool calculating;

        private void CalculateSolutions()
        {
            calculating = true;

            Mouse.SetCursor(Cursors.Wait);
            Solutions.Source = physicsEngine.CalculateSolutions();
            Mouse.SetCursor(Cursors.Arrow);

            calculating = false;

        }
    }
}
