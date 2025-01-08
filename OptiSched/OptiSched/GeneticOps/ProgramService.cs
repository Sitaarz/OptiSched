﻿using Microsoft.IdentityModel.Tokens;
using OptiSched.GeneticOps.GeneticModels;
using OptiSched.Models;
using OptiSched.Models.GeneticModels;

namespace OptiSched.GeneticOps;

public interface IProgramService
{
    List<Individual> Population { get; }
    List<Individual> CreatePopulation();
    Individual RouletteSelection(List<Individual> population);
    List<Individual> Step();
    void Run();
}

public class ProgramService : IProgramService
{
    public List<Individual> Population { get; private set; }
    public List<TimePeriod> AvailablePeriods { get; private set; }
    public static Random Random { get; } = new();
    private AppDbContext Db { get; }
    private GeneticOperatorsService GeneticOperators { get; }

    public ProgramService(AppDbContext dbContext)
    {
        GeneticOperators = new GeneticOperatorsService();
        Db = dbContext;
        GetAvailablePeriods();
        Population = CreatePopulation();
    }

    public List<Individual> CreatePopulation()
    {
        var newPopulation = new List<Individual>();

        for (var i = 0; i < AppSettings.GeneticSettings.PopulationSize; i++)
        {
            var randomIndiviudal = CreateRandomIndividual();
            newPopulation.Add(randomIndiviudal);
        }

        return newPopulation;
    }

    public Individual RouletteSelection(List<Individual> population)
    {
        // var sortedPopulation = Population.OrderBy(x => 1.0 / x.Fittness).ToList();

        var inverseFitnessSum = 0d;
        foreach (var individual in population) inverseFitnessSum += 1.0 / individual.Fittness;

        var randomFitness = Random.NextDouble() * inverseFitnessSum;

        var runningSum = 0d;

        foreach (var individual in population)
        {
            runningSum += 1.0 / individual.Fittness;
            if (runningSum >= randomFitness) return individual;
        }

        throw new NullReferenceException("The random individual is null");
    }

    public List<Individual> Step()
    {
        var numberOfIndividualsToCreate
            = Math.Ceiling((1d - AppSettings.GeneticSettings.PercentageOfPopulationToMove)
                           * AppSettings.GeneticSettings.PopulationSize);
        var numberOfElitaryIndividuals = AppSettings.GeneticSettings.PopulationSize - (int)numberOfIndividualsToCreate;
        var numberOfRooms = Db.Rooms.Count();

        List<Individual> newPopulation = new();

        newPopulation.AddRange(GetElitaryIndividuals(numberOfElitaryIndividuals));

        for (var i = 0d; i < numberOfIndividualsToCreate; i++)
        {
            var shouldCrossOver = Random.NextDouble() < AppSettings.GeneticSettings.CrossoverProbability;
            Individual newIndividual;
            if (shouldCrossOver)
            {
                var randomIndividual1 = RouletteSelection(Population);
                var randomIndividual2 = RouletteSelection(Population);
                newIndividual = GeneticOperators.Crossover(randomIndividual1, randomIndividual2);
            }
            else
            {
                var randomIndividual = RouletteSelection(Population);
                newIndividual = GeneticOperators.Mutate(randomIndividual, numberOfRooms);
            }

            newPopulation.Add(newIndividual);
        }

        return newPopulation;
    }

    public void Run()
    {
        for (var i = 0; i < AppSettings.GeneticSettings.GenerationNumber; i++)
        {
            var newPopulation = Step();
            foreach (var individual in newPopulation) individual.CountFitness(Db);

            var bestIndividual = newPopulation.OrderBy(x => x.Fittness).FirstOrDefault();

            Console.WriteLine($"bestFitness: {bestIndividual.Fittness}");
            
            Population = newPopulation;

            
            var allSchedules = Db.Schedules;
            Db.Schedules.RemoveRange(allSchedules);
            
            foreach (var meeting in bestIndividual.GeneticMeetings)
            {
                var schedule = new Schedule()
                {
                    MeetingId = meeting.Id,
                    StartTime = meeting.StartDate,
                    EndTime = meeting.EndDate,
                    RoomId = meeting.RoomId,
                };
                
                Db.Schedules.Add(schedule);
            }

            Db.SaveChanges();
        }
    }


    private List<Individual> GetElitaryIndividuals(int numberOfIndividuals)
    {
        var sortedPopulation = Population.OrderBy(x => x.Fittness);
        var elitaryIndividuals = sortedPopulation.Take(numberOfIndividuals).ToList();

        return elitaryIndividuals;
    }

    private void GetAvailablePeriods()
    {
        var availabilities = Db.Availabilities.OrderBy(x => x.StartDate).ToList();
        var periods = new List<TimePeriod>();

        if (availabilities == null || availabilities.Count == 0)
            throw new InvalidOperationException("No availabilities found");

        var currentPeriod = new TimePeriod(availabilities[0].StartDate, availabilities[0].EndDate);

        foreach (var availability in availabilities.Skip(1))
            if (availability.StartDate > currentPeriod.StartTime)
            {
                periods.Add(currentPeriod);
                currentPeriod = new TimePeriod(availability.StartDate, availability.EndDate);
            }
            else
            {
                currentPeriod.EndTime =
                    availability.EndDate > currentPeriod.EndTime
                        ? availability.EndDate
                        : currentPeriod.EndTime;
            }

        periods.Add(currentPeriod);

        if (periods.IsNullOrEmpty()) throw new NullReferenceException();

        AvailablePeriods = periods;
    }

    private TimeSpan CountTotalDuration()
    {
        var totalDuration = TimeSpan.Zero;

        if (AvailablePeriods.IsNullOrEmpty()) throw new InvalidOperationException("No available periods found");

        foreach (var priod in AvailablePeriods) totalDuration += priod.EndTime - priod.StartTime;

        return totalDuration;
    }

    private Individual CreateRandomIndividual()
    {
        var roomCount = Db.Rooms.Count();
        var totalDurationOfAvailablePeriods = CountTotalDuration().Minutes;
        var allDatabaseMeetings = Db.Meetings.ToList();

        List<GeneticMeeting> newIndividualMeetings = new();

        foreach (var meeting in allDatabaseMeetings)
        {
            var randomMinute = new TimeSpan(0, Random.Next(totalDurationOfAvailablePeriods) + 1, 0);

            foreach (var availablePeriod in AvailablePeriods)
                if (availablePeriod.EndTime - availablePeriod.StartTime >= randomMinute)
                {
                    var startOfMeeting = availablePeriod.StartTime + randomMinute;
                    var endOfMeeting = availablePeriod.EndTime + randomMinute + meeting.Duration;
                    var roomId = Random.Next(roomCount) + 1;

                    newIndividualMeetings.Add(new GeneticMeeting(meeting.UserId1, meeting.UserId2, roomId,
                        startOfMeeting, endOfMeeting));
                    break;
                }
                else
                {
                    randomMinute = randomMinute - (availablePeriod.EndTime - availablePeriod.StartTime);
                }
        }

        return new Individual(newIndividualMeetings);
    }
}