using System.Net;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Entity Framework to use SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("Server=localhost;Database=Data;User Id=SA;Password=St0rngP@ss123!;"
));

// Configure Kestrel server to listen on all IPs on port 5000
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(IPAddress.Any, 5000);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// API Endpoint to get Name and Email
app.MapGet("/users", async (ApplicationDbContext dbContext) =>
{
    var users = await dbContext.Users
        .Select(u => new { u.Name, u.Email })
        .ToListAsync();

    return users;
}).WithName("GetUsers");

app.Run();
