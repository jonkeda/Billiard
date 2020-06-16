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
        private readonly SoundManager soundManager;

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

        private void UpdateRenderer()
        {
            renderer.Update();

            if (physicsEngine.Resting)
            {
                Vector2D p = Mouse.GetPosition(Table);

                var (ballPosition, ballRadius) = physicsEngine.balls.Find(x => x.index == 0);

                renderer.DrawQueue(ballPosition, ballRadius, p);
                renderer.DrawTrajectory(ballRadius, physicsEngine.CalculateTrajectory(ballPosition, (ballPosition - p).Normalize(), ballRadius));
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
            if (!physicsEngine.Resting)
            {
                return;
            }

            Vector2D p = Mouse.GetPosition(Table);

            PBall ball = physicsEngine.balls.Last();

            Vector2D n = (ball.position - p).Normalize();
            Vector2D force = Math.Min((ball.position - p).Length, 200) * 10 * n;

            soundManager.BreakSound(p, force.Length);

            physicsEngine.ApplyForce(ball, force);
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
            if (soundManager.ToggleSound())
            {
                ToggleSoundIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeHigh;
            }
            else
            {
                ToggleSoundIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.VolumeOff;
            }
        }
        #endregion

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            CompositionTarget.Rendering += Update;

            soundManager = new SoundManager();

            physicsEngine = new PhysicsEngine();
            physicsEngine.Trigger += Trigger;
            physicsEngine.Collision += soundManager.CollisionSound;

            renderer = new Renderer(Table, Half, Full, Queue, Overlay);
            renderer.ResetAll(physicsEngine.balls);
        }
    }
}
