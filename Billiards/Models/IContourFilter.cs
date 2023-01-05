namespace Billiard.Models;

public interface IContourFilter : IAbstractFilter
{
    ContourCollection Contours { get; set; }
}