namespace OptiSched.Models;

public class Room
{
    public int Id{get;set;}
    
    public virtual Schedule Schedule{get;set;}
}