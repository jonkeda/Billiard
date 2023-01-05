using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Billiard.Physics;

public class CollisionCollection : Collection<Collision>
{
    public bool TwoDifferentBallsHit()
    {
        int? index = null;

        foreach (Collision collision in this)
        {
            if (collision.Ball != null)
            {
                if (!index.HasValue)
                {
                    index = collision.Ball.index;
                }
                else if (index.Value != collision.Ball.index)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public Geometry AsGeometry()
    {
        if (Count <= 2)
        {
            return null;
        }

        StreamGeometry geometry = new StreamGeometry();

        using var ctx = geometry.Open();

        Collision s = this[0];
        ctx.BeginFigure(new Point(s.Position.X, s.Position.Y), false, false);
        foreach (Collision v in this.Skip(1))
        {
            ctx.LineTo(new Point(v.Position.X, v.Position.Y), true, false);
        }

        return geometry;
    }

}