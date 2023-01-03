namespace Billiards.Base.Drawings;

public class FormattedText
{
    public string Text { get; }
    public int Size { get; }
    public Brush Brush { get; }
    public double PointSize { get; }

    public FormattedText(string text, int size, Brush brush, double pointSize)
    {
        Text = text;
        Size = size;
        Brush = brush;
        PointSize = pointSize;
    }
    /*
         *                FormattedText formattedText = new(
                    ball.Index.ToString(),
                   // CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    32,
                    Brushes.AntiqueWhite, 1.25);

         * 
         */
}