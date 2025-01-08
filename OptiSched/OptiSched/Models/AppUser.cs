using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace OptiSched.Models;

public class AppUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "nvarchar(150)")]
    public string Name { get; set; }

    [PersonalData]
    [Column(TypeName = "nvarchar(150)")]
    public string Surname { get; set; }

    public virtual ICollection<Meeting> Meetings { get; set; }

    public virtual ICollection<Availabilitie> Availabilities { get; set; }
}