using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Billiard.Physics;
using Billiard.Renderer.Effects;
using Billiard.Utilities;

namespace Billiard.Renderer
{
    class Renderer
    {
        private readonly Canvas tableCanvas;

        private readonly Dictionary<PBall, Rectangle> tableBalls = new();

        private readonly Image overlay, solutions;

        private readonly float length;
        private readonly float width;

        public Renderer(Canvas _tableCanvas, Image _overlay, Image _solutions, float _length, float _width)
        {
            tableCanvas = _tableCanvas;
            overlay = _overlay;
            length = _length;
            width = _width;
            solutions = _solutions;
        }

        #region Common

        public void Update()
        {
            UpdateBalls();

            overlay.Source = null;
        }

        public void ResetAll(List<PBall> balls)
        {
            InitBalls(balls);
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

        public void UpdateBalls()
        {
            foreach (KeyValuePair<PBall, Rectangle> pair in tableBalls)
            {
                var (ball, rect) = pair;

                Panel.SetZIndex(rect, (int)((ball.position - new Vector2(length / 2f, width / 2f)).Length() * 1000f));
                rect.Effect.SetValue(Ball3DEffect.Rot0Property, ball.rotation.Column0);
                rect.Effect.SetValue(Ball3DEffect.Rot1Property, ball.rotation.Column1);
                rect.Effect.SetValue(Ball3DEffect.Rot2Property, ball.rotation.Column2);
                rect.Effect.SetValue(Ball3DEffect.PositionProperty, new Point3D(ball.position.X, 0, ball.position.Y));
                rect.RenderTransform = new TranslateTransform(ball.position.X - ball.r * 4, ball.position.Y - ball.r * 4);
            }
        }

        #endregion

        #region Trajectory

        public void DrawTrajectory(
            //float ballRadius, 
            Geometry geometry, Force force)
        {
            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                drawingContext.PushClip(new RectangleGeometry(new Rect(new Point(0,0), new Point(length, width))));
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), null, new Rect(0, 0, length, width));
                
                Pen dotted = new Pen(Brushes.White, 2)
                {
                    DashStyle = DashStyles.Dash
                };
/*
                drawingContext.DrawLine(dotted, (trajectory.Origin + (trajectory.Hit - trajectory.Origin).Normalize() * ballRadius).AsPoint(), trajectory.Hit.AsPoint());
                drawingContext.DrawEllipse(null, dotted, trajectory.Hit.AsPoint(), ballRadius, ballRadius);
                drawingContext.DrawLine(new Pen(Brushes.White, 2), trajectory.Hit.AsPoint(), (trajectory.Hit + trajectory.Normal * 40f).AsPoint());

                DrawBallTrajectories(drawingContext);
*/
                drawingContext.DrawGeometry(null, dotted, geometry);
                
                Pen forceColor = new Pen(Brushes.GreenYellow, 2)
                {
                    DashStyle = DashStyles.Dot
                };

                drawingContext.DrawLine(forceColor, force.Position.AsPoint(), (force.Position - force.VectorPower).AsPoint());
                drawingContext.DrawEllipse(null, forceColor, force.Position.AsPoint(), 50, 50);

                drawingContext.Close();
            }

            overlay.Source = new DrawingImage(visual.Drawing);
        }
        #endregion

        #region Queue
/*        public void DrawQueue(Vector2 ballPosition, float ballRadius, Vector2 p)
        {
            //Show(queue);
            Vector2 n = (ballPosition - p).Normalize();
            float angle = MathV.GetAngle(n);

            TransformGroup transform = new TransformGroup();
            transform.Children.Add(new RotateTransform(angle, 210, 40));
            transform.Children.Add(new TranslateTransform(
                ballPosition.X - 100 - ballRadius - length / 2 - Math.Min((ballPosition - p).Length, 200) * n.X,
                ballPosition.Y - width / 2 - Math.Min((ballPosition - p).Length, 200) * n.Y)
            );

            queue.RenderTransform = transform;
        }
*/        
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

        private Rectangle GenerateRect(PBall ball, float lightZ, float showPlane, float intensity)
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
                    LightPosition = new Point3D(length / 2, lightZ, width / 2),
                    ShowPlane = showPlane,
                    Hardness = 2.2f,
                    Intensity = intensity,
                },
            };

            rect.IsHitTestVisible = false;
            rect.CacheMode = new BitmapCache();
            rect.LayoutTransform.Freeze();

            return rect;
        }

/*        public void Hide(FrameworkElement element, bool hitTest = false)
        {
            element.Visibility = Visibility.Hidden;
            if (hitTest) element.IsHitTestVisible = false;
        }
*/

        #endregion

/*        private void DrawBallTrajectories(DrawingContext drawingContext)
        {
            foreach (KeyValuePair<PBall, Rectangle> pair in tableBalls)
            {
                if (pair.Key.DrawTrajectory 
                    && pair.Key.Color != null)
                {
                    Geometry geometry = pair.Key.PositionsAsGeometry();
                    if (geometry != null)
                    {
                        Pen pen = pair.Key.Pen;
                        drawingContext.DrawGeometry(null, pen, geometry);

                        foreach (Collision collision in pair.Key.Collisions)
                        {
                            Pen collidedPen = collision.Ball?.Pen;
                            if (collidedPen == null)
                            {
                                collidedPen = pen;
                            }
                            drawingContext.DrawEllipse(null, collidedPen, collision.Position.AsPoint(), pair.Key.r, pair.Key.r);
                        }
                    }
                }
            }
        }
*/    }
}
