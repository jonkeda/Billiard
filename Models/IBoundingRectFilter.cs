using System.Drawing;

namespace Billiard.Models;

public interface IAbstractFilter
{}

public interface IBoundingRectFilter : IAbstractFilter
{
    Rectangle BoundingRect { get; set; }
}