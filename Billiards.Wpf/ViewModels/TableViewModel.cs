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
using Rect = System.Windows.Rect;

namespace Billiards.Wpf.ViewModels
{
    public class TableViewModel : ViewModel
    {
        public VideoDeviceViewModel VideoDevice { get; }

        public float Length { get; } = 2100 * 0.9f; // 1800; // 1200;
        public float Width { get; } = 1050 * 0.9f; // 900;
        public float Radius { get; } 

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
            Output = DrawBalls(result.Balls);
             SolutionsImage = DrawSolutions(result.Problems);
        }

        private DrawingImage? DrawSolutions(PhysicsEngine.ProblemCollection? problems)
        {
            if (problems == null)
            {
                return null;
            }

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                float width = this.Length;
                float height = this.Width;

                drawingContext.PushClip(new RectangleGeometry(
                    new Rect(new Point(0, 0),
                        new Point(width, height))));
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), null,
                    new Rect(0, 0, width, height));

                foreach (PhysicsEngine.Problem problem in problems)
                {
                    Pen color = new Pen(LineBrushByBallColor(problem.Color), 3);
                    color.DashStyle = DashStyles.Dot;

                    foreach (Solution solution in problem.Solutions)
                    {
                        Geometry geometry = CollisionsAsGeometry(solution.Collisions);
                        drawingContext.DrawGeometry(null, color, geometry);
                    }
                }

                drawingContext.Close();
            }

            return new DrawingImage(visual.Drawing);
        }

        private void DrawSolution(DrawingContext drawingContext, Solution solution)
        {
            
        }

        public Geometry CollisionsAsGeometry(CollisionCollection collisions)
        {
            if (collisions.Count <= 2)
            {
                return null;
            }

            StreamGeometry geometry = new StreamGeometry();

            using var ctx = geometry.Open();

            Collision s = collisions[0];
            ctx.BeginFigure(new Point(s.Position.X, s.Position.Y), false, false);
            foreach (Collision v in collisions.Skip(1))
            {
                ctx.LineTo(new Point(v.Position.X, v.Position.Y), true, false);
            }

            return geometry;
        }


        private DrawingImage DrawBalls(ResultBallCollection balls)
        {
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                float width = this.Length;
                float height = this.Width;

                drawingContext.PushClip(new RectangleGeometry(
                    new Rect(new Point(0, 0),
                        new Point(width, height))));
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)), null,
                    new Rect(0, 0, width, height));

                DrawBalls(balls, 
                    drawingContext);

                drawingContext.Close();
            }

            return new DrawingImage(visual.Drawing);
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
                return new Point(0, 0);
            }
            return new System.Windows.Point(p.Value.X * Length, p.Value.Y * Width);
        }

        private void DrawBalls(ResultBallCollection balls, 
            DrawingContext drawingContext)
        {
            Pen pen = new Pen(Brushes.Black, 3)
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

            Pen examplePen = new Pen(Brushes.GreenYellow, 5)
            {
                DashStyle = DashStyles.Solid
            };
            PathFigure figure = new PathFigure
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
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext drawingContext = visual.RenderOpen())
            {
                drawingContext.PushClip(new RectangleGeometry(new Rect(new Point(0, 0), new Point(Length, Width))));

                DrawLines(drawingContext);

                drawingContext.Close();
            }
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)Length, (int)Width, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BackGroundImage = bitmap;
        }

        private void DrawLines(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.Green, null, new Rect(0, 0, Length, Width));

            Pen pen = new Pen(Brushes.SaddleBrown, 2)
            {
                DashStyle = DashStyles.Dot
            };


            float width4 = Width / 4;
            for (int i = 1; i < 4; i++)
            {
                drawingContext.DrawLine(pen, new Point(0, i * width4), new Point(Length, i * width4));
            }

            float length8 = Length / 8;
            for (int i = 1; i < 8; i++)
            {
                drawingContext.DrawLine(pen, new Point(i * length8, 0), new Point(i * length8, Width));
            }
        }


    }
}
