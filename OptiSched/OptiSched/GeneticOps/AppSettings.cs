namespace OptiSched.GeneticOps;

public class GeneticSettings
{
    public int PopulationSize { get; set; }
    public int GenerationNumber { get; set; }
    public double PercentageOfPopulationToMove { get; set; }
    public double CrossoverProbability { get; set; }
    public int MinutesRangeToMoveDuringMutation { get; set; }
}



public static class AppSettings
{
    public static GeneticSettings GeneticSettings { get; }

    static AppSettings()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true);

        var configuration = builder.Build();


        GeneticSettings = configuration.GetSection("GeneticSettings").Get<GeneticSettings>() ?? throw new NullReferenceException();
    }
}