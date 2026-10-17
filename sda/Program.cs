using DotNetEnv;
using Neo4j.Driver;
using Neo4jClient.Cypher;
using sda.Configurations;
using sda.Services;

Env.Load(options: new LoadOptions(setEnvVars: true, clobberExistingVars: true));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<Neo4jService>();
builder.Services.AddSingleton(Neo4jConfiguration.Aura);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
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

app.MapGet("/api/neo4j/test", async (Neo4jService neo4jService) =>
{
    var client = await neo4jService.GetClientAsync();

    var result = await (await neo4jService.CypherAsync())
        .Return(() => new { value = Return.As<int>("1") })
        .ResultsAsync;

    return Results.Ok(new
    {
        message = "Neo4j AuraDB konekcija je uspešna!",
        connected = result.First().value == 1
    });
});

app.Run();
