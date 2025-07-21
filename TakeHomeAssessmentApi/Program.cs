using Infrastructure;
using Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Redis configuration
var redisHost = builder.Configuration["REDIS__HOST"] ?? "localhost";
var redisPort = builder.Configuration["REDIS__PORT"] ?? "6379";
var redisConnectionString = $"{redisHost}:{redisPort},abortConnect=false";

// Registrar la conexión Redis usando ConnectAsync y await
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    return ConnectionMultiplexer.ConnectAsync(redisConnectionString).GetAwaiter().GetResult();
});

// DI for services
builder.Services.AddInfrastructureServiceservices(builder.Configuration);
builder.Services.AddServices(builder.Configuration);

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
