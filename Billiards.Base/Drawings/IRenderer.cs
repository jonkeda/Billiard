using OpenCvSharp;

namespace Billiards.Base.Drawings;

public interface IPlatformBrush
{

}

public interface IPlatformPen
{

}

public interface IPlatformColor
{

}

public interface IRenderer
{
    void DrawEllipse(Brush? brush, Pen? pen, Point2f center, float radiusX, float radiusY);
}