using OpenCvSharp;

namespace Billiards.Base.Filters;

public interface IBoundingRectFilter : IAbstractFilter
{
    Rect BoundingRect { get; set; }
}