using CourseManagement.API;
using CourseManagement.API.Extensions;
using CourseManagement.Infrastructure.ApplicationData;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

builder.Services
    .AddCustomAuth(configuration)
    .AddEFServices(configuration)
    .AddCustomServices();

builder.Services
    .AddOpenApi()
    .AddSerilogLogging(configuration)
    .AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbSeeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await dbSeeder.DbSeed();
}

app.UseSerilogRequestLogging();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

