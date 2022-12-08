using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Media;

namespace Utilities;

class Vector2Collection : Collection<Vector2>
{

    public Geometry AsGeometry()
    {
        if (Count <= 2)
        {
            return null;
        }

        StreamGeometry geometry = new StreamGeometry();

        using var ctx = geometry.Open();

        Vector2 s = this[0];
        ctx.BeginFigure(new Point(s.X, s.Y), false, false);
        foreach (Vector2 v in this.Skip(1))
        {
            ctx.LineTo(new Point(v.X, v.Y), true, false);
        }

        return geometry;
    }
}