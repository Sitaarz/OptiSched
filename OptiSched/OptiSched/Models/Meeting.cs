namespace OptiSched.Models;

public class Meeting
{
    public int Id { get; set; }
    public TimeSpan Duration { get; set; }
    public string UserId1 { get; set; }
    public string UserId2 { get; set; }

    public virtual AppUser User1 { get; set; }
    public virtual AppUser User2 { get; set; }

    public virtual Schedule Schedule { get; set; }
}