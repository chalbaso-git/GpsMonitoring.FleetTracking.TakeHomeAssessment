using Infrastructure;
using Infrastructure.PostgreSQL;
using Services;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Redis configuration
var redisHost = builder.Configuration["REDIS__HOST"] ?? "localhost";
var redisPort = builder.Configuration["REDIS__PORT"] ?? "6379";
var redisConnectionString = $"{redisHost}:{redisPort},abortConnect=false";

// Register Redis connection
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.ConnectAsync(redisConnectionString).GetAwaiter().GetResult());

// DI for services
builder.Services.AddServices(builder.Configuration);
builder.Services.AddInfrastructureServiceservices(builder.Configuration);

// PostgreSQL configuration
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL")
    ?? "Host=localhost;Port=5432;Database=fleetdb;Username=postgres;Password=postgres";

builder.Services.AddDbContext<FleetDbContext>(options =>
    options.UseNpgsql(connectionString)); // <-- Esto ahora funcionará

// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline. 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();


await app.RunAsync();
