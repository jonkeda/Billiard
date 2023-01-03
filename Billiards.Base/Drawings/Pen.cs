namespace Billiards.Base.Drawings
{
    public class Pen
    {
        public Pen(Brush brush, int thickness)
        {
            Brush = brush;
            Thickness = thickness;
        }

        public int Thickness { get; set; }

        public Brush? Brush { get; set; }

        public DashStyles DashStyle { get; set; }
        public IPlatformPen? PlatformPen { get; set; }
    }
}
