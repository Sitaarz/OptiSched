namespace OptiSched.Models.GeneticModels;

public class GeneticMeeting : Meeting
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int RoomId { get; set; }

    public GeneticMeeting(string person1Id, string person2Id, int roomId, DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate) throw new ArgumentException("Start date cannot be greater than end date");

        if (person1Id == person2Id) throw new ArgumentException("Person 1 cannot have the same person 2 id");

        if (roomId < 1) throw new ArgumentException("Room ID cannot be less than 1");

        UserId1 = person1Id;
        UserId2 = person2Id;
        RoomId = roomId;
        StartDate = startDate;
        EndDate = endDate;
    }
}