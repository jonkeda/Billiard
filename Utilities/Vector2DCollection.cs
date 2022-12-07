using System.Collections.ObjectModel;
using System.Linq;
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

        StreamGeometry geometry = new StreamGeometry();

        using var ctx = geometry.Open();

        Vector2D s = this[0];
        ctx.BeginFigure(new Point(s.x, s.y), false, false);
        foreach (Vector2D v in this.Skip(1))
        {
            ctx.LineTo(new Point(v.x, v.y), true, false);
        }

        return geometry;
    }
}