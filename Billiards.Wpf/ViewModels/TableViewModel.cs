using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Billiards.Base.Filters;
using Billiards.Base.FilterSets;
using Billiards.Base.Physics;
using Billiards.Wpf.UI;
using OpenCvSharp;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;

namespace Billiards.Wpf.ViewModels
{
    public class TableViewModel : ViewModel
    {
        public VideoDeviceViewModel VideoDevice { get; }

        public float ImageLength { get; } = 2100 * 0.9f; // 1800; // 1200;
        public float ImageWidth { get; } = 1050 * 0.9f; // 900;

        public float Length { get; } = 2000; // * 0.9f; // 1800; // 1200;
        public float Width { get; } = 1000; // * 0.9f; // 900;
        public float Radius { get; }

        public Mat? OriginalImage
        {
            get { return originalImage; }
            set { SetProperty(ref originalImage, value); }
        }

        private ImageSource? output;
        public ImageSource? Output
        {
            get { return output; }
            set { SetProperty(ref output, value); }
        }

        private ImageSource? solutionsImage;
        public ImageSource? SolutionsImage
        {
            get { return solutionsImage; }
            set { SetProperty(ref solutionsImage, value); }
        }

        private ImageSource? backGroundImage;
        private Mat? originalImage;

        public ImageSource? BackGroundImage
        {
            get { return backGroundImage; }
            set { SetProperty(ref backGroundImage, value); }
        }

        public TableViewModel(VideoDeviceViewModel videoDevice)
        {
            VideoDevice = videoDevice;

            Radius = System.MathF.Max(Length / 100, Width / 100);

            DrawBackground();
        }

        public void CaptureImage(ResultModel result)
        {
            OriginalImage = result.Image;
            Output = DrawBalls(result.Balls);
            SolutionsImage = DrawSolutions(result.Problems);
        }

        private DrawingImage? DrawSolutions(PhysicsEngine.ProblemCollection? problems)
        {
            if (problems == null)
            {
                return null;
            }

            DrawingVisual visual = new();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                float width = this.Length;
                float height = this.Width;

                drawingContext.PushClip(new RectangleGeometry(
                    new(new(0, 0),
                        new Point(width, height))));
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), null,
                    new(0, 0, width, height));

                foreach (PhysicsEngine.Problem problem in problems)
                {
                    Pen color = new(LineBrushByBallColor(problem.Color), 3)
                    {
                        DashStyle = DashStyles.Dot
                    };

                    if (problem.Solutions != null)
                    {
                        foreach (Solution solution in problem.Solutions)
                        {
                            Geometry geometry = CollisionsAsGeometry(solution.Collisions);
                            drawingContext.DrawGeometry(null, color, geometry);
                        }
                    }
                }

                drawingContext.Close();
            }

            return new(visual.Drawing);
        }

        public Geometry? CollisionsAsGeometry(CollisionCollection collisions)
        {
            if (collisions.Count <= 2)
            {
                return null;
            }

            StreamGeometry geometry = new();

            using var ctx = geometry.Open();

            Collision s = collisions[0];
            ctx.BeginFigure(new(s.Position.X, s.Position.Y), false, false);
            foreach (Collision v in collisions.Skip(1))
            {
                ctx.LineTo(new(v.Position.X, v.Position.Y), true, false);
            }

            return geometry;
        }


        private DrawingImage DrawBalls(ResultBallCollection balls)
        {
            DrawingVisual visual = new();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                float width = this.Length;
                float height = this.Width;

                drawingContext.PushClip(new RectangleGeometry(
                    new(new(0, 0),
                        new Point(width, height))));
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), null,
                    new(0, 0, width, height));

                DrawBalls(balls,
                    drawingContext);

                drawingContext.Close();
            }

            return new(visual.Drawing);
        }

        private Brush BrushByBallColor(BallColor color)
        {
            if (color == BallColor.Red)
            {
                return Brushes.DarkRed;
            }
            if (color == BallColor.Yellow)
            {
                return Brushes.Yellow;
            }
            return Brushes.White;
        }

        private Brush LineBrushByBallColor(BallColor color)
        {
            Color brushColor = Colors.White;
            if (color == BallColor.Red)
            {
                brushColor = Colors.DarkRed;
            }
            else if (color == BallColor.Yellow)
            {
                brushColor = Colors.Yellow;
            }
            Brush brush = new SolidColorBrush(brushColor);
            brush.Opacity = 0.5;
            return brush;
        }

        public System.Windows.Point ToAbsolutePoint(Point2f? p)
        {
            if (!p.HasValue)
            {
                return new(0, 0);
            }
            return new(p.Value.X * Length, p.Value.Y * Width);
        }

        private void DrawBalls(ResultBallCollection balls,
            DrawingContext drawingContext)
        {
            Pen pen = new(Brushes.Black, 3)
            {
                DashStyle = DashStyles.Solid
            };
            foreach (ResultBall ball in balls)
            {
                if (ball.TableRelativePosition.HasValue)
                {
                    drawingContext.DrawEllipse(BrushByBallColor(ball.Color), pen,
                        ToAbsolutePoint(ball.TableRelativePosition.Value), Radius, Radius);
                }
            }
        }

        private static void DrawFoundTable(List<Point> tableCornerPoints,
                    DrawingContext drawingContext)
        {
            if (tableCornerPoints.Count == 0)
            {
                return;
            }

            Pen examplePen = new(Brushes.GreenYellow, 5)
            {
                DashStyle = DashStyles.Solid
            };
            PathFigure figure = new()
            {
                IsClosed = true,
                StartPoint = tableCornerPoints[0]
            };
            foreach (Point point in tableCornerPoints.Skip(1))
            {
                figure.Segments.Add(new LineSegment(point, true));
            }

            Geometry geometry = new PathGeometry(new List<PathFigure> { figure });
            drawingContext.DrawGeometry(null, examplePen, geometry);

        }

        private void DrawBackground()
        {
            DrawingVisual visual = new();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                drawingContext.PushClip(new RectangleGeometry(new(new(0, 0), new Point(Length, Width))));

                DrawLines(drawingContext);

                drawingContext.Close();
            }
            RenderTargetBitmap bitmap = new((int)Length, (int)Width, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BackGroundImage = bitmap;
        }

        private void DrawLines(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.Green, null, new(0, 0, Length, Width));

            Pen pen = new(Brushes.SaddleBrown, 2)
            {
                DashStyle = DashStyles.Dot
            };


            float width4 = Width / 4;
            for (int i = 1; i < 4; i++)
            {
                drawingContext.DrawLine(pen, new(0, i * width4), new(Length, i * width4));
            }

            float length8 = Length / 8;
            for (int i = 1; i < 8; i++)
            {
                drawingContext.DrawLine(pen, new(i * length8, 0), new(i * length8, Width));
            }
        }


    }
}
