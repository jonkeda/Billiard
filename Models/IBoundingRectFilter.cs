using System.Drawing;

namespace Billiard.Models;

public interface IBoundingRectFilter : IAbstractFilter
{
    Rectangle BoundingRect { get; set; }
}