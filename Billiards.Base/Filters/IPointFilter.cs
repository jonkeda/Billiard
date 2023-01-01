using OpenCvSharp;

namespace Billiards.Base.Filters;

public interface IPointFilter : IAbstractFilter
{
    Point2f Point2f { get; set; }
}