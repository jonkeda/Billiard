namespace Billiards.Base.Drawings;

public class PathGeometry : Geometry
{
    public PathGeometry(List<PathFigure> pathFigures)
    {
        PathFigures = pathFigures;
    }

    public List<PathFigure> PathFigures { get; set; }
}