using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using ReactApp1.Server.Data;
using ReactApp1.Server.DTO;
using ReactApp1.Server.Interfaces;
using ReactApp1.Server.Repositories;
using ReactApp1.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region MyServices
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresDatabaseOptions")));

builder.Services.AddLogging();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddTransient<IPlayerService, PlayerService>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();

builder.Services.Configure<RttfLinks>(
    builder.Configuration.GetSection(nameof(RttfLinks)));

builder.Services.Configure<HangfireJobsSettings>(
    builder.Configuration.GetSection("HangfireJobs"));

var connectionString = builder.Configuration.GetConnectionString("PostgresDatabaseOptions");
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString)));
builder.Services.AddHangfireServer();

#endregion

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();

var hangfireJobsConfig = builder.Configuration.GetSection("HangfireJobs");
var cronSchedule = hangfireJobsConfig["PostTournamentsPlayersStatsCron"];
var startDay = hangfireJobsConfig.GetValue<int>("PostTournamentsPlayersStatsStartDay");
var endDay = hangfireJobsConfig.GetValue<int>("PostTournamentsPlayersStatsEndDay");
RecurringJob.AddOrUpdate<IPlayerService>(
    "PostTournamentsPlayersStats",
    service => service.PostTournamentsPlayersStatsNearbyDays(startDay, endDay),
    cronSchedule);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
