using CourseManagement.API;
using CourseManagement.API.Extensions;
using CourseManagement.Business.Factories;
using CourseManagement.Business.Options;
using CourseManagement.Infrastructure.ApplicationData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

builder.Services.AddOpenApi();
builder.Services.AddSerilogLogging(configuration);
builder.Services.AddEFServices(configuration);
builder.Services.AddControllers();
builder.Services.AddCustomServices();

builder.Services.Configure<AuthOptions>(configuration.GetSection(nameof(AuthOptions)));

var authSection = configuration.GetSection(nameof(AuthOptions));
var authOptions = authSection.Get<AuthOptions>()
            ?? throw new InvalidOperationException("JWT configuration section is missing or invalid.");

var secretKey = Encoding.UTF8.GetBytes(authOptions.Secret);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = TokenValidationParametersFactory.Create(authOptions);
    });

builder.Services.AddAuthorization();

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