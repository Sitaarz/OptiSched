using OptiSched.Models;
using OptiSched.Models.GeneticModels;

namespace OptiSched.GeneticOps.GeneticModels;

public interface IIndividual
{
    List<GeneticMeeting> GeneticMeetings { get; }
    int Fittness { get; set; }
    int CountFitness(AppDbContext Db);
}

public class Individual : IIndividual
{
    private IIndividual _individualImplementation;

    public Individual(List<GeneticMeeting> geneticMeetings)
    {
        GeneticMeetings = geneticMeetings;
    }

    public List<GeneticMeeting> GeneticMeetings { get; set; }
    public int Fittness { get; set; }

    public int CountFitness(AppDbContext Db)
    {
        Fittness = 0;
        var usersAvailabilitest =
            Db.Availabilities.GroupBy(availability => availability.AppUserId)
                .ToDictionary(key => key.Key,
                    value => value.OrderBy(x => x.StartDate).ToList());

        foreach (var meeting in GeneticMeetings)
        {
            var firstPersonAvailability = usersAvailabilitest[meeting.UserId1];
            var secondPersonAvailability = usersAvailabilitest[meeting.UserId2];
            var firstPersonTimeAdded = false;
            var secondPersonTimeAdded = false;

            foreach (var availability in firstPersonAvailability)
            {
                if (availability.StartDate >= meeting.StartDate && availability.StartDate <= meeting.EndDate)
                {
                    var difference = availability.StartDate - meeting.StartDate;
                    Fittness += (int)difference.TotalMinutes;
                    firstPersonTimeAdded = true;
                    break;
                }

                if (availability.EndDate >= meeting.StartDate && availability.EndDate <= meeting.EndDate)
                {
                    var difference = meeting.EndDate - availability.EndDate;
                    Fittness += (int)difference.TotalMinutes;
                    firstPersonTimeAdded = true;
                    break;
                }

                if (availability.StartDate <= meeting.StartDate && availability.EndDate >= meeting.EndDate)
                {
                    firstPersonTimeAdded = true;
                    break;
                }
            }

            if (!firstPersonTimeAdded) Fittness += (int)(meeting.EndDate - meeting.StartDate).TotalMinutes;


            foreach (var availability in secondPersonAvailability)
            {
                if (availability.StartDate >= meeting.StartDate && availability.StartDate <= meeting.EndDate)
                {
                    var difference = availability.StartDate - meeting.StartDate;
                    Fittness += (int)difference.TotalMinutes;
                    secondPersonTimeAdded = true;
                    break;
                }

                if (availability.EndDate >= meeting.StartDate && availability.EndDate <= meeting.EndDate)
                {
                    var difference = meeting.EndDate - availability.EndDate;
                    Fittness += (int)difference.TotalMinutes;
                    secondPersonTimeAdded = true;
                    break;
                }

                if (availability.StartDate <= meeting.StartDate && availability.EndDate >= meeting.EndDate)
                {
                    secondPersonTimeAdded = true;
                    break;
                }
            }

            if (!secondPersonTimeAdded) Fittness += (int)(meeting.EndDate - meeting.StartDate).TotalMinutes;
        }

        return Fittness;
    }
}