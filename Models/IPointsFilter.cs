using System.Collections.Generic;

namespace Billiard.Models;

public interface IPointsFilter : IAbstractFilter
{
    List<System.Windows.Point> Points { get; set; }
}

public interface IPointFilter : IAbstractFilter
{
    System.Windows.Point Point { get; set; }
}