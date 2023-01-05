namespace Billiards.Web.Shared;

public class Table
{
    public Table(PointCollection corners)
    {
        Corners = corners;
    }
    public PointCollection Corners { get; }
}