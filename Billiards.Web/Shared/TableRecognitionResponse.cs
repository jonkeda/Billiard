namespace Billiards.Web.Shared;

public class TableRecognitionResponse
{
    public TableRecognitionResponse(Table? table, BallCollection? balls, LogCollection? log)
    {
        Table = table;
        Balls = balls;
        Log = log;
    }

    public Table? Table { get; set; }

    public BallCollection? Balls { get; set; }

    public LogCollection? Log { get; set; }
}