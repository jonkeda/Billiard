using System.Collections.Generic;
using System.Drawing;
using Emgu.CV.Util;

namespace Billiard.Camera.vision.Geometries;

public static class VectorOfPointExtensions
{
    public static List<Point> AsList(this VectorOfPoint vector)
    {
        List<Point> points = new List<Point>();
        foreach (Point point in vector.ToArray())
        {
            points.Add(point);
        }
        return points;
    }
}