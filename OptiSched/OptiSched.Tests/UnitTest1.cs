using Microsoft.EntityFrameworkCore;
using OptiSched.GeneticOps;
using OptiSched.Models;

namespace OptiSched.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source = C:\\Users\\kryst\\RiderProjects\\OptiSchedRoot\\OptiSched\\OptiSched\\app.db")
            .Options;
        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            // var newAvailability = new Availabilitie
            // {
            //     AppUserId = "ff44fffc-bdb6-4103-a17c-14dfb73aeea1", // Przykładowe ID użytkownika
            //     StartDate = new DateTime(2025, 01, 08, 08, 00, 00),
            //     EndDate = new DateTime(2025, 01, 08, 17, 00, 00)
            // };

            // Dodanie nowego obiektu do DbSet
            // context.Availabilities.Add(newAvailability);
            //
            // // Zapisanie zmian w bazie danych
            // context.SaveChanges();
            //
            var service = new ProgramService(context);

            service.Run();

            Console.WriteLine(context.Users.Count());
        }
    }
}