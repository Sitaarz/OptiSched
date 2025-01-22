using OptiSched.Models;
using OptiSched.Models.GeneticModels;

namespace OptiSched.GeneticOps.GeneticModels;

public interface IIndividual
{
    List<GeneticMeeting> GeneticMeetings { get; }
    int Fitness { get; set; }
    int CalculateFitness(AppDbContext Db);
}

public class ZeroFitnessException : Exception
{
    public ZeroFitnessException()
    {
    }

    public ZeroFitnessException(string message)
        : base(message)
    {
    }

    public ZeroFitnessException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class Individual : IIndividual
{
    private IIndividual _individualImplementation;

    public Individual()
    {
        GeneticMeetings = new List<GeneticMeeting>();
    }

    public Individual(List<GeneticMeeting> geneticMeetings)
    {
        GeneticMeetings = geneticMeetings;
    }

    public List<GeneticMeeting> GeneticMeetings { get; set; }
    public int Fitness { get; set; }

    public int CalculateFitness(AppDbContext Db)
    {
        Fitness = 0;
        var usersAvailabilitest =
            Db.Availabilities.GroupBy(availability => availability.AppUserId)
                .ToDictionary(key => key.Key,
                    value => value.OrderBy(x => x.StartDate).ToList());

        foreach (var meeting in GeneticMeetings)
        {
            var firstPersonAvailability = usersAvailabilitest[meeting.UserId1];
            var secondPersonAvailability = usersAvailabilitest[meeting.UserId2];

            var firstPersonError = CalculateErrorByPersonAvailability(firstPersonAvailability, meeting);
            var secondPersonError = CalculateErrorByPersonAvailability(secondPersonAvailability, meeting);
            Fitness += firstPersonError;
            Fitness += secondPersonError;
        }

        var multiplicationError = CalculateErrorByHavingAnotherMeetingInTheSameTime(GeneticMeetings);
        Fitness += multiplicationError;

        var roomError = CalculateRoomError(GeneticMeetings);
        Fitness += roomError;

        if (Fitness == 0) throw new ZeroFitnessException();
        return Fitness;
    }

    private int CalculateErrorByPersonAvailability(List<Availability> personAvailabilite, GeneticMeeting meeting)
    {
        foreach (var availability in personAvailabilite)
        {
            var overlapStart = Max(availability.StartDate, meeting.StartDate);
            var overlapEnd = Min(availability.EndDate, meeting.EndDate);
            if (overlapStart < overlapEnd)
            {
                var overlap = overlapEnd - overlapStart;
                return (int)(meeting.Duration - overlap).TotalMinutes;
            }
        }

        return 1000;
    }

    private int CalculateErrorByHavingAnotherMeetingInTheSameTime(List<GeneticMeeting> meetings)
    {
        var error = 0;

        for (var i = 0; i < meetings.Count - 1; i++)
        for (var j = i + 1; j < meetings.Count; j++)
        {
            
            var overlapStart = Max(meetings[i].StartDate, meetings[j].StartDate);
            var overlapEnd = Min(meetings[i].EndDate, meetings[j].EndDate);

            if (overlapStart < overlapEnd && meetings[i].RoomId == meetings[j].RoomId) error += (int)overlapEnd.Subtract(overlapStart).TotalMinutes ;
        }

        return error;
    }

    private int CalculateRoomError(List<GeneticMeeting> meetings)
    {
        var meetingsPerRoom
            = meetings.GroupBy(meeting => meeting.RoomId)
                .ToDictionary(key => key.Key, values => values.ToList());

        int error = 0;
        
        foreach (List<GeneticMeeting> selectedMeetings in meetingsPerRoom.Values)
        {
            for (int i = 0; i < selectedMeetings.Count - 1; i++)
            {
                for (int j = i + 1; j < selectedMeetings.Count; j++)
                {
                    var overlapStart = Max(meetings[i].StartDate, meetings[j].StartDate);
                    var overlapEnd = Min(meetings[i].EndDate, meetings[j].EndDate);

                    if (overlapStart < overlapEnd) error += (int)overlapEnd.Subtract(overlapStart).TotalMinutes ;
                }
            }
        }

        return error;
    }

    private static DateTime Max(DateTime a, DateTime b)
    {
        return a > b ? a : b;
    }

    private static DateTime Min(DateTime a, DateTime b)
    {
        return a < b ? a : b;
    }
}