using Hangfire;
using Hangfire.Storage.SQLite;
using HangfireSqliteExample.Data;
using HangfireSqliteExample.Repository;
using HangfireSqliteExample.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlite(connectionString));

builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<ITimeService, TimeService>();


builder.Services.AddHangfire(cfg => cfg
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSQLiteStorage(connectionString.Split('=').Last().Trim(),new SQLiteStorageOptions
    {
        QueuePollInterval = TimeSpan.FromMilliseconds(10),
        InvisibilityTimeout = TimeSpan.FromMinutes(5)
    }));

builder.Services.AddHangfireServer(opt => opt
    .SchedulePollingInterval = TimeSpan.FromSeconds(1)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

//db migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    using (var conn = db.Database.GetDbConnection())
    {
        conn.Open();
    }
}
//

app.UseAuthorization();
app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<ITimeService>("recurrent-printtime-job", service=> service.PrintTime(),Cron.Hourly);

app.MapControllers();

app.Run();
