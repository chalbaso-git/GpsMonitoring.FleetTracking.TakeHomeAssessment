using Infrastructure;
using Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Redis configuration
var redisHost = builder.Configuration["REDIS__HOST"] ?? "localhost";
var redisPort = builder.Configuration["REDIS__PORT"] ?? "6379";
var redisConnectionString = $"{redisHost}:{redisPort},abortConnect=false";

// Register Redis connection
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConnectionString));

// DI for services
builder.Services.AddServices(builder.Configuration);
builder.Services.AddInfrastructureServiceservices(builder.Configuration);

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

app.Run();
