namespace Billiard.Models;

public interface IPointFilter : IAbstractFilter
{
    System.Windows.Point Point { get; set; }
}