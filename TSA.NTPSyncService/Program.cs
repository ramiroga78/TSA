using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.DependencyInjection;
using TSA.Infrastructure.Interfaces;
using TSA.Infrastructure.Mappings;
using TSA.Infrastructure.Services;
using TSA.NTPSyncService.Settings;

namespace TSA.NTPSyncService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var logInViewer = config["WorkerSettings:LogInEventViewer"];
            if (logInViewer == "1")
            {
                var logName = config["WorkerSettings:LogInEventViewerName"];
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.EventLog(logName, manageEventSource: true)
                    .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();
            }

            try
            {
                Log.Information("Starting up the service");
                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "There was a problem starting the service");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            //var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            //var dbConexionString = config["ConnectionStrings:DefaultConnection"];

            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    services.AddRepository();
                    services.AddSingleton<IAlertService, AlertService>();
                    services.AddSingleton<IMessageService, MessageService>();
                    services.AddSingleton<IDeltaService, DeltaService>();
                    services.AddSingleton<INTPServerService, NTPServerService>();
                    services.AddAutoMapper(typeof(AutoMapperProfile));
                    services.AddDbContext<TSADbContext>(options =>
                    {
                        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                    }, ServiceLifetime.Singleton);
                    WorkerSettings options = configuration.GetSection("WorkerSettings").Get<WorkerSettings>();
                    services.AddSingleton(options);
                    services.AddHostedService<Worker>();
                })
                .UseSerilog();
        }
    }
}
