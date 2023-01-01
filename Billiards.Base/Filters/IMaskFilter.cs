using OpenCvSharp;

namespace Billiards.Base.Filters;

public interface IMaskFilter : IAbstractFilter
{
    Mat Mask { get; set; }
}