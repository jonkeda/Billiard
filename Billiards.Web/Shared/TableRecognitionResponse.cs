namespace Billiards.Web.Shared;

public class TableRecognitionResponse
{
    public TableRecognitionResponse(Table? table, BallCollection? balls)
    {
        Table = table;
        Balls = balls;
    }

    public Table? Table { get; set; }

    public BallCollection? Balls { get; set; }
}