namespace OptiSched.GeneticOps.GeneticModels;

public class TimePeriod
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public TimePeriod(DateTime startTime, DateTime endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }
}