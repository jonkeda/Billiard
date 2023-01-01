namespace Billiards.Base.Filters;

public interface IContourFilter : IAbstractFilter
{
    ContourCollection Contours { get; set; }
}