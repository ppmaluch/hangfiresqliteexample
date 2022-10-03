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

//register hangfire to services
builder.Services.AddHangfire(cfg => cfg
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSQLiteStorage(connectionString.Split('=').Last().Trim(),new SQLiteStorageOptions
    {
        QueuePollInterval = TimeSpan.FromMilliseconds(10),
        InvisibilityTimeout = TimeSpan.FromMinutes(5)
    }));

/*
 * creating hangfire server instance to handle jobs on repository, this can be implemented on a dedicated server,
   in this case we handle it in the same api server

   for educative purposes we established a polling interval to the server to seek scheduled jobs every second,
   for production this options should be leaved by default (15 seconds)
 */
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

//use hangfire dashboard to visualize job statuses and related data
app.UseHangfireDashboard();

//if we want to add a recurrent job that needs to be initialized and scheduled from the beginning
RecurringJob.AddOrUpdate<ITimeService>("recurrent-printtime-job", service=> service.PrintTime(),Cron.Hourly);

app.MapControllers();

app.Run();
