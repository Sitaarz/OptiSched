using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OptiSched.Domain;
using OptiSched.GeneticOps;
using OptiSched.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Adres aplikacji Angular
            .AllowAnyMethod() // Pozwól na dowolne metody HTTP (GET, POST, etc.)
            .AllowAnyHeader() // Pozwól na dowolne nagłówki
            .AllowCredentials(); // Jeśli używasz ciasteczek/uwierzytelniania
    });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(y =>
    {
        y.SaveToken = false;
        y.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = false,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:JWTsecret"]!)
            ),
            ValidateIssuer = false,
            ValidateAudience = false
        };
        y.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token successfully validated");
                var claims = context.Principal?.Claims.Select(c => $"{c.Type}: {c.Value}");
                foreach (var claim in claims ?? Enumerable.Empty<string>()) Console.WriteLine($"Claim: {claim}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build()
);

builder.Services
    .AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SqLiteConnectionString"))
        .LogTo(Console.WriteLine, LogLevel.None));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGroup("/api").MapIdentityApi<AppUser>();

app.MapPost("/api/signup", [AllowAnonymous] async (
    UserManager<AppUser> userManager,
    [FromBody] UserRegistrationModelDto userRegistrationModel
) =>
{
    var user = new AppUser()
    {
        UserName = userRegistrationModel.Email,
        Email = userRegistrationModel.Email,
        Name = userRegistrationModel.Name,
        Surname = userRegistrationModel.Surname
    };
    var result = await userManager.CreateAsync(
        user,
        userRegistrationModel.Password);
    if (result.Succeeded)
        return Results.Ok(result);
    return Results.BadRequest(result);
});

app.MapPost("/api/signin", [AllowAnonymous] async
(UserManager<AppUser> userManager,
    [FromBody] UserLoginModelDto userLoginModel) =>
{
    var user = await userManager.FindByNameAsync(userLoginModel.Email);
    if (user != null && await userManager.CheckPasswordAsync(user, userLoginModel.Password))
    {
        var signInKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:JWTsecret"]!));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new("UserId", user.Id)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);
        return Results.Ok(token);
    }

    return Results.BadRequest(new { message = "Email or password is incorrect" });
});

app.MapGet("/user", [AllowAnonymous] async Task<IResult> (ClaimsPrincipal user, UserManager<AppUser> manager) =>
{
    var userId = user.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
    if (userId == null) return Results.Unauthorized();

    var userDetails = await manager.FindByIdAsync(userId);
    if (userDetails == null) return Results.Unauthorized();

    return Results.Ok(new
    {
        name = userDetails?.Name,
        surnamme = userDetails?.Surname,
        email = userDetails?.Email
    });
});

app.MapGet("/schedule",
    [AllowAnonymous] async Task<IResult> (ClaimsPrincipal user, UserManager<AppUser> manager, AppDbContext db) =>
    {
        var userId = user.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        if (userId == null) return Results.Unauthorized();

        var userDetails = await manager.FindByIdAsync(userId);
        if (userDetails == null) return Results.Unauthorized();

        var schedule = db.Schedules.ToList();
        return Results.Ok(schedule);
    });

app.MapGet("/getUsers",
    [AllowAnonymous] async Task<IResult> (AppDbContext db) =>
    {
        var users = db.AppUsers;
        var usersDto = users.Select(user => new AppUserDTO()
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname
        });

        return Results.Ok(usersDto);
    });

app.MapGet("/getAvailability",
    [AllowAnonymous]
    async Task<IResult> (AppDbContext db, ClaimsPrincipal userPrincipal, UserManager<AppUser> manager) =>
    {
        var userId = userPrincipal.Claims.FirstOrDefault(claim => claim.Type == "UserId")?.Value;

        if (userId == null) return Results.Unauthorized();

        var userDetails = await manager.FindByIdAsync(userId);
        if (userDetails == null) return Results.Unauthorized();

        var avabilities = db.Availabilities.ToList().Where(avability => avability.AppUserId == userId)
            .Select(availability => new
            {
                id = availability.Id,
                start = availability.StartDate,
                end = availability.EndDate,
                title = "available"
            });

        return Results.Ok(avabilities);
    });

app.MapPost("/addMeeting", [AllowAnonymous] async Task<IResult> (AppDbContext db,
        ClaimsPrincipal user,
        [FromBody] MeetingDto meeting) =>
    {
        var userId = user.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        if (userId == null) return Results.Unauthorized();

        var meetingToAdd = new Meeting()
        {
            UserId1 = userId,
            UserId2 = meeting.GuestId,
            Duration = TimeSpan.FromHours(meeting.Duration)
        };

        db.Meetings.Add(meetingToAdd);
        db.SaveChanges();

        return Results.Created();
    }
);

app.MapPost("/addAvailability", [AllowAnonymous] async Task<IResult> (
        ClaimsPrincipal user,
        UserManager<AppUser> userManager,
        AppDbContext context,
        [FromBody] List<AvailabilityDto> userAvailability
    ) =>
    {
        var userId = user.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        if (userId == null) return Results.Unauthorized();

        var userAccessibilities = userAvailability.Select(availability => new Availability()
        {
            AppUserId = userId,
            StartDate = availability.Start,
            EndDate = availability.End
        });

        var entities = context.Availabilities.Where(e => e.AppUserId == userId).ToList();
        if (entities.Any())
        {
            context.Availabilities.RemoveRange(entities);
            context.SaveChanges();
        }

        context.Availabilities.AddRange(userAccessibilities);
        context.SaveChanges();

        return Results.Created();
    }
);

app.MapGet("/getSchedule", [AllowAnonymous] async Task<IResult> (AppDbContext db) =>
{
    var result = from schedule in db.Schedules
        join meeting in db.Meetings on schedule.MeetingId equals meeting.Id
        join user1 in db.AppUsers on meeting.UserId1 equals user1.Id
        join user2 in db.AppUsers on meeting.UserId2 equals user2.Id
        join room in db.Rooms on schedule.RoomId equals room.Id
        select new
        {
            Id = schedule.Id,
            user1Name = user1.Name,
            user1Surname = user1.Surname,
            user2Name = user2.Name,
            user2Surname = user2.Surname,
            startTime = schedule.StartTime,
            endTime = schedule.EndTime,
            roomId = schedule.RoomId
        };

    return Results.Ok(result);
});

app.MapPost("/startProgram", [AllowAnonymous] async Task<IResult> (AppDbContext db) =>
{
    var program = new ProgramService(db);
    program.Run();

    return Results.Ok();
});

app.Run();
