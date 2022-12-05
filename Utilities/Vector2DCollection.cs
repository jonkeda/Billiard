using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Utilities;

class Vector2DCollection : Collection<Vector2D>
{

    public Geometry AsGeometry()
    {
        if (this.Count <= 2)
        {
            return null;
        }

        PathFigure figure = null;
        foreach (Vector2D v in this)
        {
            if (figure == null)
            {
                figure = new PathFigure
                {
                    IsClosed = false,
                    IsFilled = false,
                    StartPoint = new Point(v.x, v.y)
                };
            }
            else
            {
                figure.Segments.Add(new LineSegment(new Point(v.x, v.y), true));
            }
        }

        List<PathFigure> figures = new List<PathFigure>
        {
            figure
        };
        
        return new PathGeometry(figures);
    
    }
}