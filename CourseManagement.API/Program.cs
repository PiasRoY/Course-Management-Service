using CourseManagement.API;
using CourseManagement.API.Extensions;
using CourseManagement.API.Handlers;
using CourseManagement.Business.Extensions;
using CourseManagement.Infrastructure.ApplicationData;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services
    .AddCustomAuth(configuration)
    .AddEFServices(configuration)
    .AddHangfireService(configuration)
    .AddCustomServices()
    .AddCustomMiddlewares()
    .AddCustomOptions();

builder.Services
    .AddTransient<DbSeeder>();

builder.Services
    .AddOpenApi()
    .AddSerilogLogging(configuration)
    .AddHttpContextAccessor()
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

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

app.UseHttpsRedirection();

app.UseHsts();

app.UseAuthentication();

app.UseMiddleware<SerilogLogContextMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

