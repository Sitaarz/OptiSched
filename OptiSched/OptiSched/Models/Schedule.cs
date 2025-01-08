namespace OptiSched.Models;

public class Schedule
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int RoomId { get; set; }

    public virtual Meeting Meeting { get; set; }
    public virtual Room Room { get; set; }
}