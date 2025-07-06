using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Charger ocelot.json
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Ajouter les services
builder.Services.AddHealthChecks();

// Ajouter Ocelot SANS Consul pour tester d'abord
builder.Services.AddOcelot();

// Ajouter CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Middleware de débogage
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();
app.MapHealthChecks("/health");

// Middleware Ocelot
await app.UseOcelot();

Console.WriteLine("API Gateway démarré sur http://localhost:5000");
Console.WriteLine("Testez: http://localhost:5000/health");
Console.WriteLine("Testez: http://localhost:5000/orders");

app.Run();