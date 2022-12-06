using Physics;
using Physics.Triggers;
using Render;
using Sound;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Utilities;

namespace Billiard
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private readonly PhysicsEngine physicsEngine;
        private readonly Renderer renderer;
        // private readonly SoundManager soundManager;

        private int miss = 0;

        private long t = DateTime.Now.Ticks / 10000;

        private readonly Queue<double> fpsDeltas = new Queue<double>(new double[8]);

        private string fps;
        public string FPS
        {
            get { return fps; }
            set
            {
                if (fps != value)
                {
                    fps = value;
                    OnPropertyChanged();
                }
            }
        }

        private string score;
        public string Score
        {
            get { return score; }
            set
            {
                if (score != value)
                {
                    score = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Gameloop
        private void Update(object sender, EventArgs e)
        {
            UpdateUI();
            UpdatePhysics();
            UpdateRenderer();
        }

        private void UpdateUI()
        {
            fpsDeltas.Dequeue();
            fpsDeltas.Enqueue(1000.0d / (DateTime.Now.Ticks / 10000 - t));

            FPS = Math.Round(fpsDeltas.Sum() / 8).ToString() + " FPS";

            Score = "Punkte: " + CalculateScore();
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

        private void UpdateRenderer()
        {
            renderer.Update();

            if (physicsEngine.Resting)
            {
                Vector2D p = Mouse.GetPosition(Table);

                var (ballPosition, ballRadius) = GetCueBall();

                renderer.DrawQueue(ballPosition, ballRadius, p);

                CalculateForce(out var force);

                physicsEngine.Calculate(force);

                // (ballPosition - p).Normalize()
                renderer.DrawTrajectory(ballRadius, 
                    physicsEngine.CalculateTrajectory(ballPosition, physicsEngine.Force.VectorPower, ballRadius),
                    physicsEngine.CalculateForce(ballPosition));
            }
        }
        #endregion

        #region Gameeventhandlers and utitilies
        private void Won()
        {
            renderer.Show(WonHelper, true);
        }

        private void Lost()
        {
            renderer.Show(LooseScreen, true);
        }

        private int CalculateScore()
        {
            int sum = 0;

            for (int i = 0; i < Math.Min(physicsEngine.HalfBalls.Count, physicsEngine.FullBalls.Count); i++)
            {
                sum += physicsEngine.HalfBalls[i].index * physicsEngine.FullBalls[i].index;
            }

            for (int i = 1; i < miss; i++)
            {
                sum -= 2 * i;
            }

            return sum;
        }

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
        }

        private PBall CalculateForce(out Vector2D force)
        {
            PBall ball = GetCueBall();

            Vector2D n = physicsEngine.Force.Vector;
            force = Math.Min(physicsEngine.Force.Power, 200) * 10 * n;

            
            return ball;
        }

        private void Trigger(object sender, TriggerEvent e)
        {
            if (e.ball.index == 0)
            {
                e.ball.velocity = new Vector2D(0, 0);
                e.ball.position = new Vector2D(273, 547 / 2);
                miss++;
                return;
            }

            if (physicsEngine.balls.Count == 2)
            {
                Won();
            }
            else if (e.ball.index == 8)
            {
                Lost();
            }

            PBallWithG newBall = new PBallWithG(e.ball.index, 20, new Vector2D(100, 100), e.ball.velocity);

            physicsEngine.TransferBall(e.ball, newBall);
            renderer.AddSideBall(newBall);
            renderer.RemoveBall(e.ball);
        }
        #endregion

        #region UIEventhandlers
        private void RestartGame(object sender = null, RoutedEventArgs e = null)
        {
            renderer.Hide(LooseScreen);
            miss = 0;
            physicsEngine.Init();
            renderer.ResetAll(physicsEngine.balls);
        }

        private void ToggleFullScreen(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                SizeToContent = SizeToContent.WidthAndHeight;
                WindowState = WindowState.Normal;
            }
            else
            {
                SizeToContent = SizeToContent.Manual;
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
            }
        }

        private void OpenHighscore(object sender, RoutedEventArgs e)
        {
            renderer.Show(Highscore, true);

            ScoreManager.SaveScore("xD", new Random().Next());

            Scores.ItemsSource = ScoreManager.Scores;
        }

        private void CloseHighscore(object sender, RoutedEventArgs e)
        {
            renderer.Hide(Highscore, true);
        }

        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SendHighscoreAndRestart(object sender, RoutedEventArgs e)
        {
            ScoreManager.SaveScore(PlayerName.Text, CalculateScore());
            RestartGame();
            WonHelper.Visibility = Visibility.Hidden;
            WonHelper.IsHitTestVisible = false;
        }

        private void ToggleSound(object sender, RoutedEventArgs e)
        {
/*            if (soundManager.ToggleSound())
            {
                ToggleSoundIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeHigh;
            }
            else
            {
                ToggleSoundIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeOff;
            }
*/        }
        #endregion

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            CompositionTarget.Rendering += Update;

            //soundManager = new SoundManager();

            physicsEngine = new PhysicsEngine(GameType.Billiart);
            physicsEngine.Trigger += Trigger;
            //physicsEngine.Collision += soundManager.CollisionSound;

            renderer = new Renderer(Table, Half, Full, Queue, Overlay, Solutions, physicsEngine.Length, physicsEngine.Width);
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
        }

        private void CalculateSolutions()
        {
            Mouse.SetCursor(Cursors.Wait);
            Solutions.Source = physicsEngine.CalculateSolutions();
            Mouse.SetCursor(Cursors.Arrow);

        }
    }
}
