using Billiards.Base.FilterSets;
using Billiards.Web.Shared;
using Microsoft.AspNetCore.Mvc;
using OpenCvSharp;
using Point = Billiards.Web.Shared.Point;

namespace Billiards.Web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PredictionController : ControllerBase
    {
        [HttpPost]
        public PredictionResponse Predict(PredictionRequest image)
        {

            return new PredictionResponse();
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