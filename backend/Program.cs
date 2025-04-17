using backend.Commands.ApproveCarrierCommand;
using backend.Commands.CreateCarrierCommand;
using backend.Commands.DeleteCarrierCommand;
using backend.Commands.RejectCarrierCommand;
using backend.Commands.UpdateCarrierCommand;
using backend.Queries.GetAllCarriersQuery;
using backend.Queries.SearchCarriersQuery;
using backend.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repositories
builder.Services.AddScoped<SqlCarrierRepository>();
builder.Services.AddScoped<ICarrierRepository>(sp => sp.GetRequiredService<SqlCarrierRepository>());

// Register query handlers
builder.Services.AddScoped<GetAllCarriersQueryHandler>();
builder.Services.AddScoped<SearchCarriersQueryHandler>();

// Register command handlers
builder.Services.AddScoped<CreateCarrierCommandHandler>();
builder.Services.AddScoped<UpdateCarrierCommandHandler>();
builder.Services.AddScoped<DeleteCarrierCommandHandler>();
builder.Services.AddScoped<ApproveCarrierCommandHandler>();
builder.Services.AddScoped<RejectCarrierCommandHandler>();

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