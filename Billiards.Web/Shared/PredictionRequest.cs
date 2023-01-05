namespace Billiards.Web.Shared;

public class PredictionRequest
{
    public PredictionRequest(BallCollection balls)
    {
        Balls = balls;
    }

    public BallCollection Balls { get; }
}