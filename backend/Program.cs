using backend.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Try registering the concrete implementation first
builder.Services.AddScoped<SqlCarrierRepository>();
// Then register the interface with the implementation
builder.Services.AddScoped<ICarrierRepository>(sp => sp.GetRequiredService<SqlCarrierRepository>());

var allowedOrigins = builder.Configuration.GetValue<string>("allowOrigins")!.Split(",");

builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();