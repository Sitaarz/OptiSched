namespace OptiSched.Models;

public class Availabilitie
{
    public int Id { get; set; }
    public string AppUserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public virtual AppUser AppUser { get; set; }
}