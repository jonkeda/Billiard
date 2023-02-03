namespace Billiards.Web.Shared;

public class PredictionResponse
{
    public PredictionResponse(ProblemCollection problems, LogCollection log)
    {
        Problems = problems;
        Log = log;
    }

    public ProblemCollection Problems { get; }
    
    public LogCollection? Log { get; set; }
}