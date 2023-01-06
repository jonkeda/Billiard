using OpenCvSharp;

namespace Billiards.Base.Drawings;

public interface IRenderer
{
    void DrawEllipse(Brush? brush, Pen? pen, Point2f center, float radiusX, float radiusY);
    void PushClip(Geometry geometry);
    void DrawRectangle(Brush? brush, Pen? pen, Rect2f rect);
    void DrawLine(Pen? pen, Point2f p1, Point2f p2);
    void DrawText(FormattedText formattedText, Point2f position);
    void DrawGeometry(Brush? brush, Pen? pen, Geometry geometry);
}