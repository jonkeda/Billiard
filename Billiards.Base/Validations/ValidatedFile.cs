using Billiards.Base.FilterSets;

namespace Billiards.Base.Validations;

public class ValidatedFile
{
    public string FileName { get; set; } = null!;

    public ValidatedBallCollection Balls { get; set; } = new();
    public bool Found { get; set; }
    public bool Equal { get; set; }

    public void SetBalls(ResultBallCollection resultBalls)
    {
        bool isEqual = false;
        foreach (var resultBall in resultBalls)
        {
            isEqual |=  SetBall(resultBall);
        }
        Equal = isEqual;
    }

    private bool SetBall(ResultBall resultBall)
    {
        ValidatedBall? ball = Balls.FirstOrDefault(b => b.Color == resultBall.Color);
        int x = resultBall.TableRelativePosition.HasValue ? (int)(resultBall.TableRelativePosition.Value.X * 2000) : -1;
        int y = resultBall.TableRelativePosition.HasValue ? (int)(resultBall.TableRelativePosition.Value.Y * 1000) : -1;

        if (ball == null)
        {
            ball = new ValidatedBall(resultBall.Color);
            Balls.Add(ball);
        }
        ball.ResultX = x;
        ball.ResultY = y;
        return ball.X == ball.ResultX
               && ball.Y == ball.ResultY;
    }
}