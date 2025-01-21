using OptiSched.Domain;

namespace OptiSched.Models;

public class UserRegistrationModelDto : UserLoginModelDto
{
    public string Name { get; set; }
    public string Surname { get; set; }
}