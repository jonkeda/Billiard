namespace Billiards.Base.Filters;

public interface IContourFilter : IAbstractFilter
{
    ContourCollection? Contours { get; set; }
}

public interface ISingleContourFilter : IAbstractFilter
{
    public IContourFilter? ContourFilter { get; set; }
}
