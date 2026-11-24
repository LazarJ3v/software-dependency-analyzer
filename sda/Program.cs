using DotNetEnv;
using sda.Configurations;
using sda.Services;

// Env.Load(options: new LoadOptions(setEnvVars: true, clobberExistingVars: true));
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(Neo4jConfiguration.Aura);
builder.Services.AddSingleton<Neo4jService>();

builder.Services.AddSingleton<ProjectService>();
builder.Services.AddSingleton<PackageService>();
builder.Services.AddSingleton<VulnerabilityService>();
builder.Services.AddSingleton<AnalysisService>();
builder.Services.AddSingleton<ProjectGraphService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// Provera konekcije sa Neo4j bazom pri startu aplikacije
var neo4j = app.Services.GetRequiredService<Neo4jService>();
await neo4j.VerifyConnectivityAsync();
app.Logger.LogInformation("Neo4j connection established.");

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// app.MapGet("/api/neo4j/test", async (Neo4jService neo4jService) =>
// {
//     var records = await neo4jService.ReadAsync("RETURN 1 AS value");

//     return Results.Ok(new
//     {
//         message = "Neo4j AuraDB konekcija je uspešna!",
//         connected = records[0]["value"].As<int>() == 1
//     });
// });

app.Run();