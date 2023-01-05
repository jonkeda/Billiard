using System.Globalization;

namespace Billiards.Web.Shared;

public class Point
{
    public Point()
    { }

    public Point(float x, float y)
    {
        X = x;
        Y = y;
    }

    public float X { get; set; }
    public float Y { get; set; }

    public override string ToString()
    {
        return X.ToString("F1", CultureInfo.InvariantCulture) + "," +
               Y.ToString("F1", CultureInfo.InvariantCulture);
    }
}