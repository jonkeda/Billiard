using OpenCvSharp;

namespace Billiards.Base.Drawings;

public interface IRenderer
{
    void DrawEllipse(Brush? brush, Pen? pen, Point2f center, float radiusX, float radiusY);
}