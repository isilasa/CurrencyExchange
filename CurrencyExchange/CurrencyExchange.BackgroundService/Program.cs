using CurrencyExchange.BackgroundService.Jobs;
using CurrencyExchange.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddQuartz(q =>
{
    q.AddJob<UpdateCurrencyJob>(j => j
       .StoreDurably()
       .WithIdentity("UpdateCurrencyJob"));
    q.AddTrigger(t => t
        .WithIdentity("UpdateCurrencyJob-trigger")
        .ForJob("UpdateCurrencyJob")
        .WithSimpleSchedule(o => o.WithIntervalInHours(24).RepeatForever())
        .StartAt(DateBuilder.TodayAt(6, 0, 0)));
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
