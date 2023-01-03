using OpenCvSharp;

namespace Billiards.Base.Drawings;

public class TextShape : AbstractShape
{
    public FormattedText FormattedText { get; }
    public Point2f Position { get; }

    public TextShape(FormattedText formattedText, Point2f position)
    {
        FormattedText = formattedText;
        Position = position;
    }
}