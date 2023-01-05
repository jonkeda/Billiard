namespace Billiard.Models;

public class BallResult
{
    public Contour Contour { get; set; }
    public double Mean { get; set; }
    public double Max { get; set; }
    public int Index { get; set; }
    public BallColor Color { get; set; }
}