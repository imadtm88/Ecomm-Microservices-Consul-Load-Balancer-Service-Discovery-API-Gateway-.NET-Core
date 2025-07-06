using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using Consul;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<IProductRepository, MongoProductRepository>();
builder.Services.AddHealthChecks(); // Health check pour Consul

var app = builder.Build();

// Configure le pipeline HTTP
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

//Consul Registration
var consulConfig = app.Configuration.GetSection("ConsulConfig");

var consulClient = new ConsulClient(config =>
{
    config.Address = new Uri(consulConfig["Address"]);
});

var registration = new AgentServiceRegistration()
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
