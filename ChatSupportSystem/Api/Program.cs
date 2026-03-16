using Application.Services;
using Application.Workers;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Queue;
using Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.Configure<Redis>(builder.Configuration.GetSection("Redis"));

// DI
builder.Services.AddSingleton<InMemoryQueue>();
builder.Services.AddSingleton<IDateTimeProvider, Infrastructure.Time.SystemDateTimeProvider>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<AgentService>();
builder.Services.AddScoped<ChatSessionService>();
builder.Services.AddScoped<AgentAssignmentService>();
builder.Services.AddScoped<CapacityService>();

builder.Services.AddHostedService<ChatDispatcherWorker>();
builder.Services.AddHostedService<PollMonitorWorker>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- SEED DATA EXECUTION ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();

    try
    {
        // Apply migrations if needed
        db.Database.Migrate();

    } catch (Exception ex)
    { 
    }
    // Seed initial data
    SeedData.Initialize(db);
}
// ----------------------------

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();