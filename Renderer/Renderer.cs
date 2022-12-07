﻿using Effects;
using Physics;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Billiard.Physics;
using Utilities;

namespace Render
{
    class Renderer
    {
        private readonly Canvas tableCanvas;

        private readonly Dictionary<PBall, Rectangle> tableBalls = new Dictionary<PBall, Rectangle>();

        private readonly Image queue, overlay, solutions;

        private readonly double length;
        private readonly double width;

        public Renderer(Canvas _tableCanvas, Image _queue, Image _overlay, Image _solutions, double _length, double _width)
        {
            tableCanvas = _tableCanvas;
            queue = _queue;
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
            Hide(queue);
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

                Panel.SetZIndex(rect, (int)((ball.position - new Vector2D(length / 2, width / 2)).Length * 1000));
                rect.Effect.SetValue(Ball3DEffect.Rot0Property, ball.rotation.Column0);
                rect.Effect.SetValue(Ball3DEffect.Rot1Property, ball.rotation.Column1);
                rect.Effect.SetValue(Ball3DEffect.Rot2Property, ball.rotation.Column2);
                rect.Effect.SetValue(Ball3DEffect.PositionProperty, new Point3D(ball.position.x, 0, ball.position.y));
                rect.RenderTransform = new TranslateTransform(ball.position.x - ball.r * 4, ball.position.y - ball.r * 4);
            }
        }

        #endregion

        #region Trajectory

        public void DrawTrajectory(double ballRadius, Trajectory trajectory, Force force)
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

                drawingContext.DrawLine(dotted, trajectory.Origin + (trajectory.Hit - trajectory.Origin).Normalize() * ballRadius, trajectory.Hit);
                drawingContext.DrawEllipse(null, dotted, trajectory.Hit, ballRadius, ballRadius);
                drawingContext.DrawLine(new Pen(Brushes.White, 2), trajectory.Hit, trajectory.Hit + trajectory.Normal * 40);

                DrawBallTrajectories(drawingContext);

                Pen forceColor = new Pen(Brushes.GreenYellow, 2)
                {
                    DashStyle = DashStyles.Dot
                };

                drawingContext.DrawLine(forceColor, force.Position, force.Position - force.VectorPower);
                drawingContext.DrawEllipse(null, forceColor, force.Position, 50, 50);

                drawingContext.Close();
            }

            overlay.Source = new DrawingImage(visual.Drawing);
        }
        #endregion

        #region Queue
        public void DrawQueue(Vector2D ballPosition, double ballRadius, Vector2D p)
        {
            //Show(queue);
            Vector2D n = (ballPosition - p).Normalize();
            double angle = MathV.GetAngle(n);

            TransformGroup transform = new TransformGroup();
            transform.Children.Add(new RotateTransform(angle, 210, 40));
            transform.Children.Add(new TranslateTransform(
                ballPosition.x - 100 - ballRadius - length / 2 - Math.Min((ballPosition - p).Length, 200) * n.x,
                ballPosition.y - width / 2 - Math.Min((ballPosition - p).Length, 200) * n.y)
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
                    LightPosition = new Point3D(length / 2, lightZ, width / 2),
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

        private void DrawBallTrajectories(DrawingContext drawingContext)
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
                            drawingContext.DrawEllipse(null, collidedPen, collision.Position, pair.Key.r, pair.Key.r);
                        }
                    }
                }
            }
        }
    }
}
