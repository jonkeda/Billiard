using Billiards.Base.FilterSets;
using Billiards.Web.Shared;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;

namespace Billiards.Web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageDataController : ControllerBase
    {
        [HttpPost]
        public TableDetectionResult DetectTable(ImageData image)
        {
            var bytes = Convert.FromBase64String(image.Data);

            CaramboleDetector detector = new();

            Mat mat = Mat.ImDecode(bytes);

            ResultModel result = new ResultModel
            {
                Image = mat,
                Detector = detector,
                Now = DateTime.Now
            };

            detector.ApplyFilters(result);

            BallCollection balls = new BallCollection();
            foreach (ResultBall resultBall in result.Balls)
            {
                Ball ball = new Ball(ConvertColor(resultBall.Color), null, null);
                balls.Add(ball);
            }

            Table table = new Table(null, null, null, null);

            return new TableDetectionResult(null, balls);
        }

        public Billiards.Web.Shared.BallColor ConvertColor(Billiards.Base.Filters.BallColor color)
        {
            return (Billiards.Web.Shared.BallColor) color;
        }
    }
}