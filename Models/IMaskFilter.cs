using Emgu.CV;

namespace Billiard.Models;

public interface IMaskFilter : IAbstractFilter
{
    Mat Mask { get; set; }
}