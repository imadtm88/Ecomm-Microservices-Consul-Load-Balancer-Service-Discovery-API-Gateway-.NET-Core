using Application.Interfaces;
using Application.Services;
using Consul;
using Infrastructure.Data;
using Infrastructure.Kafka;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database SQL Server
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));

// Services
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<IOrderRepository, SqlOrderRepository>();

builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
    );

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
builder.Services.AddSingleton<KafkaProducer>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();

// Consul Registration
var consulConfig = app.Configuration.GetSection("ConsulConfig");

var consulClient = new ConsulClient(config =>
{
    config.Address = new Uri(consulConfig["Address"]);
});

var registration = new AgentServiceRegistration
{
    ID = consulConfig["ServiceId"],
    Name = consulConfig["ServiceName"],
    Address = consulConfig["ServiceAddress"],
    Port = int.Parse(consulConfig["ServicePort"]),
    Check = new AgentServiceCheck
    {
        HTTP = $"http://{consulConfig["ServiceAddress"]}:{consulConfig["ServicePort"]}/health",
        Interval = TimeSpan.FromSeconds(10),
        Timeout = TimeSpan.FromSeconds(5),
        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)
    }
};

await consulClient.Agent.ServiceDeregister(registration.ID);
await consulClient.Agent.ServiceRegister(registration);

app.Lifetime.ApplicationStopping.Register(() =>
{
    consulClient.Agent.ServiceDeregister(registration.ID).Wait();
});

app.Run();
