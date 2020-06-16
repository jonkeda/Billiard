using Effects;
using Physics;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Utilities;

namespace Render
{
    class Renderer
    {
        private readonly Canvas tableCanvas;
        private readonly Canvas fullCanvas;
        private readonly Canvas halfCanvas;

        private readonly Dictionary<PBall, Rectangle> halfBalls = new Dictionary<PBall, Rectangle>();
        private readonly Dictionary<PBall, Rectangle> fullBalls = new Dictionary<PBall, Rectangle>();
        private readonly Dictionary<PBall, Rectangle> tableBalls = new Dictionary<PBall, Rectangle>();

        private readonly Image queue, overlay;

        public Renderer(Canvas _tableCanvas, Canvas _fullCanvas, Canvas _halfCanvas, Image _queue, Image _overlay)
        {
            tableCanvas = _tableCanvas;
            fullCanvas = _fullCanvas;
            halfCanvas = _halfCanvas;
            queue = _queue;
            overlay = _overlay;
        }

        #region Common

        public void Update()
        {
            UpdateSideBalls();
            UpdateBalls();

            overlay.Source = null;
            Hide(queue);
        }

        public void ResetAll(List<PBall> balls)
        {
            InitBalls(balls);
            ResetSideBalls();
        }

        #endregion

        #region Sideballs

        public void UpdateSideBalls()
        {
            foreach (KeyValuePair<PBall, Rectangle> pair in fullBalls)
            {
                UpdateSideBall(pair, true);
            }

            foreach (KeyValuePair<PBall, Rectangle> pair in halfBalls)
            {
                UpdateSideBall(pair, false);
            }
        }

        private void UpdateSideBall(KeyValuePair<PBall, Rectangle> pair, bool full)
        {
            var (ball, rect) = pair;

            rect.Effect.SetCurrentValue(Ball3DEffect.Rot0Property, ball.rotation.Column0);
            rect.Effect.SetCurrentValue(Ball3DEffect.Rot1Property, ball.rotation.Column1);
            rect.Effect.SetCurrentValue(Ball3DEffect.Rot2Property, ball.rotation.Column2);

            rect.RenderTransform = new TranslateTransform(ball.position.x - ball.r * 4, ball.position.y - ball.r * 4);

            rect.Effect.SetCurrentValue(Ball3DEffect.PositionProperty, new Point3D(ball.position.x + (full ? -1 : 1) * 970, 0, ball.position.y));
        }

        public void AddSideBall(PBall ball)
        {
            Rectangle rect = GenerateRect(ball, 2000, 0, 50000000);
            Panel.SetZIndex(rect, 1);

            if (ball.index < 8)
            {
                fullBalls.Add(ball, rect);
                fullCanvas.Children.Add(rect);
            }
            else
            {
                halfBalls.Add(ball, rect);
                halfCanvas.Children.Add(rect);
            }
        }

        public void ResetSideBalls()
        {
            halfCanvas.Children.Clear();
            halfBalls.Clear();

            fullCanvas.Children.Clear();
            fullBalls.Clear();
        }
        #endregion

        #region Tableballs
        public void InitBalls(List<PBall> balls)
        {
            tableBalls.Clear();
            tableCanvas.Children.Clear();

            foreach (var ball in balls)
            {
                Rectangle rect = GenerateRect(ball, 500, 1, 1250000);

                tableBalls.Add(ball, rect);

                tableCanvas.Children.Add(rect);
            };
        }

        public void RemoveBall(PBall ball)
        {
            tableCanvas.Children.Remove(tableBalls[ball]);
            tableBalls.Remove(ball);
        }

        public void UpdateBalls()
        {
            foreach (KeyValuePair<PBall, Rectangle> pair in tableBalls)
            {
                var (ball, rect) = pair;

                Panel.SetZIndex(rect, (int)((ball.position - new Vector2D(485, 273.5)).Length * 1000));
                rect.Effect.SetValue(Ball3DEffect.Rot0Property, ball.rotation.Column0);
                rect.Effect.SetValue(Ball3DEffect.Rot1Property, ball.rotation.Column1);
                rect.Effect.SetValue(Ball3DEffect.Rot2Property, ball.rotation.Column2);
                rect.Effect.SetValue(Ball3DEffect.PositionProperty, new Point3D(ball.position.x, 0, ball.position.y));
                rect.RenderTransform = new TranslateTransform(ball.position.x - ball.r * 4, ball.position.y - ball.r * 4);
            }
        }

        #endregion

        #region Trajectory
        public void DrawTrajectory(double ballRadius, Trajectory trajectory)
        {
            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), null, new Rect(0, 0, 970, 547));

                Pen dotted = new Pen(Brushes.White, 2)
                {
                    DashStyle = DashStyles.Dash
                };

                drawingContext.DrawLine(dotted, trajectory.Origin + (trajectory.Hit - trajectory.Origin).Normalize() * ballRadius, trajectory.Hit);
                drawingContext.DrawEllipse(null, dotted, trajectory.Hit, ballRadius, ballRadius);
                drawingContext.DrawLine(new Pen(Brushes.White, 2), trajectory.Hit, trajectory.Hit + trajectory.Normal * 40);
                drawingContext.Close();
            }

            overlay.Source = new DrawingImage(visual.Drawing);
        }
        #endregion

        #region Queue
        public void DrawQueue(Vector2D ballPosition, double ballRadius, Vector2D p)
        {
            Show(queue);
            Vector2D n = (ballPosition - p).Normalize();
            double angle = MathV.GetAngleToX(n);

            TransformGroup transform = new TransformGroup();
            transform.Children.Add(new RotateTransform(angle, 210, 40));
            transform.Children.Add(new TranslateTransform(
                ballPosition.x - 100 - ballRadius - 970 / 2 - Math.Min((ballPosition - p).Length, 200) * n.x,
                ballPosition.y - 547 / 2 - Math.Min((ballPosition - p).Length, 200) * n.y)
            );

            queue.RenderTransform = transform;
        }
        #endregion

        #region Helpers
        private Brush GenerateBrush(PBall ball)
        {
            BitmapImage texture = ball.texture;
            ImageBrush brush = new ImageBrush()
            {
                ImageSource = texture,
                Stretch = Stretch.Fill,
            };

            RenderOptions.SetCachingHint(brush, CachingHint.Cache);

            return brush;
        }

        private Rectangle GenerateRect(PBall ball, double lightZ, double showPlane, double intensity)
        {
            Rectangle rect = new Rectangle()
            {
                ClipToBounds = true,
                Width = ball.r * 8,
                Height = ball.r * 8,
                Fill = GenerateBrush(ball),
                Effect = new Ball3DEffect()
                {
                    Size = new Point(ball.r * 8, ball.r * 8),
                    Radius = ball.r,
                    LightPosition = new Point3D(970 / 2, lightZ, 547 / 2),
                    ShowPlane = showPlane,
                    Hardness = 2.2,
                    Intensity = intensity,
                },
            };

            rect.IsHitTestVisible = false;
            rect.CacheMode = new BitmapCache();
            rect.LayoutTransform.Freeze();

            return rect;
        }

        public void Hide(FrameworkElement element, bool hitTest = false)
        {
            element.Visibility = Visibility.Hidden;
            if (hitTest) element.IsHitTestVisible = false;
        }

        public void Show(FrameworkElement element, bool hitTest = false)
        {
            element.Visibility = Visibility.Visible;
            if (hitTest) element.IsHitTestVisible = true;
        }

        public void ToggleVisibility(FrameworkElement element, bool hitTest = false)
        {
            element.Visibility = element.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            if (hitTest) element.IsHitTestVisible = element.Visibility != Visibility.Visible;
        }
        #endregion
    }
}
