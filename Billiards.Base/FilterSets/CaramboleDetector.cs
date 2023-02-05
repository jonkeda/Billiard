using Billiards.Base.Filters;
using OpenCvSharp;

namespace Billiards.Base.FilterSets;

public class CaramboleDetector
{
    public bool DrawImage { get; }

    public CaramboleDetector(bool drawImage)
    {
        FilterSets = new FilterSetCollection(drawImage);
        DrawImage = drawImage;
        FilterSets.Clear();

        var table = FilterSets.AddSet(new TableDetectorSet());
        CornerDetectorSet corner = FilterSets.AddSet(new CornerDetectorSet(table.OriginalFilter, table.FoundFilter));
        var ball = FilterSets.AddSet(new BallDetectorSet(corner.ResultFilter()));
        BallResultFilter = ball.BallResultFilter;
        PointsFilter = corner.PointsFilter;
    }

    public BallResultFilter BallResultFilter { get; }
    public IPointsFilter PointsFilter { get; set; }
    public FilterSetCollection FilterSets { get; }

    public ResultModel ApplyFilters(Mat image, string? filePath)
    {
        if (filePath == null)
        {
            BallResultFilter.SaveResult = false;
        }
        else
        {
            BallResultFilter.SaveResult = true;
            BallResultFilter.Folder = Path.GetDirectoryName(filePath);
            BallResultFilter.Filename = Path.GetFileName(filePath);
        }
        return ApplyFilters(image);
    }

    public ResultModel ApplyFilters(Mat image)
    {
        Resize(image);

        ResultModel result = new()
        {
            Image = image,
            Detector = this
        };

        FilterSets.ApplyFilters(image);

        if (BallResultFilter.ResultMat == null
            || PointsFilter.Points == null)
        {
            return result;
        }

        Mat frame = result.Image;
        List<Point2f> dest = PointsFilter.Points;
        List<Point2f> src = new List<Point2f>(new[]
            {
                new Point2f(0, 0),
                new Point2f(frame.Width, 0),
                new Point2f(frame.Width, frame.Height),
                new Point2f(0, frame.Height)
            }
        );
        Mat warpingMat = Cv2.GetPerspectiveTransform(src, dest);
        List<Point2f> relativeCorners = new List<Point2f>();
        foreach (Point2f point in PointsFilter.Points)
        {
            Point2f? newPoint = ToRelativePoint2(BallResultFilter.ResultMat, point);
            if (newPoint != null)
            {
                relativeCorners.Add(newPoint.Value);
            }
        }

        result.Corners = relativeCorners;

        result.Balls.Add(new ResultBall(BallColor.White,
            ToRelativePoint(result.Image, BallResultFilter.WhiteBallPoint),
             WarpPerspective(result.Image, warpingMat, BallResultFilter.WhiteBallPoint)));

        result.Balls.Add(new ResultBall(BallColor.Yellow,
            ToRelativePoint(result.Image, BallResultFilter.YellowBallPoint),
            WarpPerspective(result.Image, warpingMat, BallResultFilter.YellowBallPoint)));

        result.Balls.Add(new ResultBall(BallColor.Red,
            ToRelativePoint(result.Image, BallResultFilter.RedBallPoint),
            WarpPerspective(result.Image, warpingMat, BallResultFilter.RedBallPoint)));

        result.Found = HasResult(result.Balls);

        return result;
    }

    private static bool HasResult(ResultBallCollection balls)
    {
        return balls.All(b => b.TableRelativePosition.HasValue);
    }

    private static void Resize(Mat image)
    {
        if (image.Height > image.Width)
        {
            int width = (image.Width * 960) / image.Height;

            Cv2.Resize(image, image, new Size(width, 960));
        }
        else
        {
            int height = (image.Height * 960) / image.Width;
            Cv2.Resize(image, image, new Size(960, height));
        }
    }

    public static Point2f? WarpPerspective(Mat frame, Mat warpingMat,
        Point2f? point)
    {
        if (!point.HasValue)
        {
            return null;
        }

        Point2f[] points = Cv2.PerspectiveTransform(new[] { point.Value }, warpingMat);
        return ToRelativePoint2(frame, points[0]);
    }

    public static Point2f? ToRelativePoint(Mat? frame, Point2f? p)
    {
        if (frame == null
            || !p.HasValue)
        {
            return null;
        }

        if (frame.Height > frame.Width)
        {
            return new Point2f(p.Value.Y / frame.Height, 1 - (p.Value.X / frame.Width));
        }
        return new Point2f(p.Value.X / frame.Width, p.Value.Y / frame.Height);
    }

    public static Point2f? ToRelativePoint2(Mat? frame, Point2f? p)
    {
        if (frame == null
            || !p.HasValue)
        {
            return null;
        }

        /*        if (frame.Height > frame.Width)
                {
                    return new Point2f(p.Value.Y / frame.Height, 1 - (p.Value.X / frame.Width));
                }
        */
        return new Point2f(p.Value.X / frame.Width, p.Value.Y / frame.Height);
    }

}