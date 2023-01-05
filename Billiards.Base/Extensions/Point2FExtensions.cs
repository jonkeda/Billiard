using OpenCvSharp;

namespace Billiards.Base.Extensions;

public static class Point2FExtensions
{
    #region As

    public static Point2f AsPoint2f(this Point p)
    {
        return new Point2f(p.X, p.Y);
    }

    #endregion
}