using System.Windows;

namespace Billiard.Camera.vision.Geometries;

public static class RectangleExtensions
{
    public static System.Windows.Rect AsRect(this System.Drawing.Rectangle r)
    {
        return new Rect(new System.Windows.Point(r.X, r.Y), new System.Windows.Size(r.Size.Width, r.Size.Height));
    }
}