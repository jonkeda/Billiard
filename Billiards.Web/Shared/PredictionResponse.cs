namespace Billiards.Web.Shared;

public class PredictionResponse
{
    public PredictionResponse(ProblemCollection problems)
    {
        Problems = problems;
    }
    
    public ProblemCollection Problems { get; }
}