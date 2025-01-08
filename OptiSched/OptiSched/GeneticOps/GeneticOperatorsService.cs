using Microsoft.VisualBasic;
using OptiSched.GeneticOps.GeneticModels;
using OptiSched.Models.GeneticModels;

namespace OptiSched.GeneticOps;

internal interface IGeneticOperators
{
    Individual Mutate(Individual individual, int roomNumber);
    Individual Crossover(Individual individual1, Individual individual2);
}

public class GeneticOperatorsService : IGeneticOperators
{
    private Random Random;

    public GeneticOperatorsService()
    {
        Random = new Random();
    }

    public Individual Mutate(Individual individual, int roomNumber)
    {
        var meetingsNumber = individual.GeneticMeetings.Count;

        var randomMeetingNumber = Random.Next(meetingsNumber);
        var randomRoomNumber = Random.Next(randomMeetingNumber) + 1;

        var randomTimeSpan
            = TimeSpan.FromMinutes(Random.Next(0, AppSettings.GeneticSettings.MinutesRangeToMoveDuringMutation + 1));
        var shouldPlusOrMinus = Random.Next(2) == 0;

        var choosenMeeting = individual.GeneticMeetings[randomMeetingNumber];
        var newStartTime
            = shouldPlusOrMinus ? choosenMeeting.StartDate + randomTimeSpan : choosenMeeting.StartDate - randomTimeSpan;
        var newEndTime
            = shouldPlusOrMinus ? choosenMeeting.EndDate + randomTimeSpan : choosenMeeting.EndDate - randomTimeSpan;

        var newMeeting
            = new GeneticMeeting(choosenMeeting.UserId1, choosenMeeting.UserId2, randomRoomNumber, newStartTime,
                newEndTime);

        List<GeneticMeeting> newMeetings = new(individual.GeneticMeetings);
        newMeetings[randomMeetingNumber] = newMeeting;

        var newIndividual = new Individual(newMeetings);
        return newIndividual;
    }

    public Individual Crossover(Individual individual1, Individual individual2)
    {
        var meetingsNumber = individual1.GeneticMeetings.Count;
        var randomMeetingNumber = Random.Next(meetingsNumber) + 1;

        List<GeneticMeeting> meetings = new();
        var meetingIds = new List<int>();

        meetings.AddRange(individual1.GeneticMeetings.Take(randomMeetingNumber));
        meetingIds = meetings.Select(meeting => meeting.Id).ToList();

        foreach (var meeting in individual2.GeneticMeetings)
            if (!meetingIds.Contains(meeting.Id))
            {
                meetingIds.Add(meeting.Id);
                meetings.Add(meeting);
            }

        return new Individual(meetings);
    }
}