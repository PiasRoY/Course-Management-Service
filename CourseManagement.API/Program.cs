using CourseManagement.API;
using CourseManagement.API.Extensions;
using CourseManagement.API.Handlers;
using CourseManagement.Infrastructure.ApplicationData;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services
    .AddCustomAuth(configuration)
    .AddEFServices(configuration)
    .AddCustomServices()
    .AddCustomMiddlewares()
    .AddCustomOptions();

builder.Services
    .AddOpenApi()
    .AddSerilogLogging(configuration)
    .AddControllers();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbSeeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await dbSeeder.DbSeed();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();

app.UseAuthentication();

app.UseMiddleware<SerilogLogContextMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

