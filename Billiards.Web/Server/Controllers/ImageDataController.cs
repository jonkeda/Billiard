using Billiards.Base.FilterSets;
using Billiards.Web.Shared;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;
using Point = Billiards.Web.Shared.Point;

namespace Billiards.Web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecognitionController : ControllerBase
    {
        [HttpPost("Image")]
        public TableRecognitionResponse DetectTableImage(TableRecognitionRequest image)
        {
            byte[] bytes = Convert.FromBase64String(image.Data);
            MemoryStream stream = new MemoryStream(bytes);
            return DetectTable(stream);
        }

        [HttpPost("Stream")]
        public TableRecognitionResponse DetectTableStream([FromForm] IEnumerable<IFormFile> files)
        {
            if (files != null)
            {
                IFormFile? file = files.FirstOrDefault();
                if (file == null)
                {
                    return new TableRecognitionResponse(null, null);
                }
                return DetectTable(file.OpenReadStream());
            }

            return new TableRecognitionResponse(null, null);
        }

        private TableRecognitionResponse DetectTable(Stream stream)
        {
            CaramboleDetector detector = new();

            Mat mat = Mat.FromStream(stream, ImreadModes.AnyColor);
            //Mat mat = Mat.ImDecode(bytes);
            //Mat mat = Cv2.ImRead(@"C:\Temp\Billiards\Ok\Ok\Ok\20221222_215004_HDR.jpg");

            ResultModel result = detector.ApplyFilters(mat);

            BallCollection balls = new ();
            foreach (ResultBall resultBall in result.Balls)
            {
                Ball ball = new (ConvertColor(resultBall.Color), 
                    ConvertPoint(resultBall.ImageRelativePosition), 
                    ConvertPoint(resultBall.TableRelativePosition));
                balls.Add(ball);
            }

            PointCollection corners = new ();
            if (result.Corners != null)
            {
                foreach (Point2f corner in result.Corners)
                {
                    Point? point = ConvertPoint(corner);
                    if (point != null)
                    {
                        corners.Add(point);
                    }
                }
            }
            Table table = new (corners);

            return new TableRecognitionResponse(table, balls);
        }

        private static Point? ConvertPoint(Point2f? point)
        {
            if (!point.HasValue)
            {
                return null;
            }
            return new Point(point.Value.X, point.Value.Y);
        }

        private static Billiards.Web.Shared.BallColor ConvertColor(Billiards.Base.Filters.BallColor color)
        {
            return (Billiards.Web.Shared.BallColor) color;
        }



    }
}