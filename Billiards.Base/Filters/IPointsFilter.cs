using OpenCvSharp;

namespace Billiards.Base.Filters;

public interface IPointsFilter : IAbstractFilter
{
    List<Point2f> Points { get; set; }
}