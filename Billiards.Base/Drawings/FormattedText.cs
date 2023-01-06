namespace Billiards.Base.Drawings;

public class FormattedText
{
    public string Text { get; }
    public int Size { get; }
    public Brush Brush { get; }
    public double PointSize { get; }
    public IPlatformFormattedText PlatformFormattedText { get; set; }

    public FormattedText(string text, int size, Brush brush, double pointSize)
    {
        Text = text;
        Size = size;
        Brush = brush;
        PointSize = pointSize;
    }
}