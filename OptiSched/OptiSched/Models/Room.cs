using Microsoft.EntityFrameworkCore;

namespace OptiSched.Models;

public class Room
{
    public int Id { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }
}