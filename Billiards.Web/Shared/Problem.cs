namespace Billiards.Web.Shared;

public class Problem
{
    public BallColor Color { get; set; }
    public SolutionCollection Solutions { get; }

    public Problem(BallColor color, SolutionCollection solutions)
    {
        Color = color;
        Solutions = solutions;
    }
}