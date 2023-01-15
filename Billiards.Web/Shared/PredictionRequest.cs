namespace Billiards.Web.Shared;

public class PredictionRequest
{
    public PredictionRequest(BallCollection balls, BallColor cueBall)
    {
        Balls = balls;
        CueBall = cueBall;
    }

    public BallColor CueBall { get; }
    public BallCollection Balls { get; }
}