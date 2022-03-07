using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSA.NTPSyncService.Settings;
using TSA.Utilities;

namespace TSA.NTPSyncService
{
    public class Worker : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger<Worker> _logger;
        private readonly WorkerSettings _options;
        private readonly IDeltaService _deltaService;
        private readonly IMessageService _messageService;
        private readonly IAlertService _alertService;
        private readonly INTPServerService _ntpServerService;

        public Worker(IHostApplicationLifetime hostApplicationLifetime,
            ILogger<Worker> logger,
            WorkerSettings options,
            IAlertService alertService,
            IMessageService messageService,
            IDeltaService deltaService,
            INTPServerService ntpServerService)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
            _options = options;
            _alertService = alertService;
            _messageService = messageService;
            _deltaService = deltaService;
            _ntpServerService = ntpServerService;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("The service has been started...");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("The service has been stopped...");
            return base.StopAsync(cancellationToken);
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int tskDelay = int.Parse(_options.TimerLoopInSeconds) * 1000;

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                    // 1) Leer estado Servicio Mensajería
                    Response nMessagingServiceStatus = new Response();
                    nMessagingServiceStatus = Service.GetServiceStatus(_options.TSAMessagingService);

                    if (nMessagingServiceStatus.Result == "OK")
                    {

                        if (nMessagingServiceStatus.Message.Equals(ServiceControllerStatus.Stopped.ToString()))
                        {
                            _logger.LogError("{0} is not running", _options.TSAMessagingService);
                            _logger.LogError("{0}: {1} - {2}", _options.TSAMessagingService, nMessagingServiceStatus.Result, nMessagingServiceStatus.Message);

                            Response saveMessage = await Factory.FactoryMessaging.SaveMessageForServices(_alertService, _messageService,
                                _options.TSAMessagingServiceAlertId, _options.TSAMessagingService, 1);

                            if (saveMessage.Result == "OK")
                            {
                                _logger.LogInformation("Servers syncronized");
                                _logger.LogInformation("{0} Messages saved", saveMessage.Values.Where(x => x.Key == "TotalMessages").FirstOrDefault().Value);
                            }
                            else
                            {
                                _logger.LogError("SaveMessageForServices: Error trying to save Messages");
                                _logger.LogError("ERROR: {0} ", saveMessage.Message);
                            }
                        }
                        else
                        {
                            _logger.LogInformation("{0} is running", _options.TSAMessagingService);
                        }
                    }
                    else
                    {
                        _logger.LogError("ERROR: {0} ", nMessagingServiceStatus.Message);
                    }

                    // 2) Sync NTP Servers
                    var databaseServers = await _ntpServerService.GetAllNTPServers();
                    //databaseServers = databaseServers.Where(x => x.IsActive == true);

                    Response saveServers = await Factory.FactoryNTPServersSync.SyncServers(_options.NTPServersRegistrySubKey,
                                        databaseServers, _options.UpdateRegistry, _logger);
                    if (saveServers.Result == "OK")
                    {
                        _logger.LogInformation("SYNC: {0} Database Servers", saveServers.Values.Where(x => x.Key == "TotalServersDatabase").FirstOrDefault().Value);
                        _logger.LogInformation("SYNC: {0} Registry Servers", saveServers.Values.Where(x => x.Key == "TotalServersRegistry").FirstOrDefault().Value);
                    }
                    else
                    {
                        _logger.LogError("SYNC: Error trying to update NTP Servers in Registry");
                        _logger.LogError("ERROR: {0} ", saveServers.Message);
                    }

                    // 3) Deltas de Control
                    var deltas = await _deltaService.GetAllDeltas();

                    var deltasControls = deltas.Where(n => n.DeltaTypeId == 2).ToList();
                    if (deltasControls.Count > 0)
                    {
                        var databaserServer = await _ntpServerService.GetAllNTPServers();

                        Response connection = await Factory.FactoryDeltasControl.Connection(deltasControls, _options, databaserServer, _deltaService, _messageService, _logger);
                        if (connection.Result == "OK")
                        {
                            _logger.LogInformation("CONNECTION: {0} Control messages recorded", connection.Values.Where(x => x.Key == "TotalControlMessages").FirstOrDefault().Value);
                            //_logger.LogInformation("CONNECTION: {0}", connection.Message);
                            //_logger.LogInformation("CONNECTION: {0} is {1}", _options.WindowsTimeServiceName, connection.Values.Where(x => x.Key == "StopService").FirstOrDefault().Value);

                        }
                        else
                        {
                            _logger.LogError("CONNECTION: Error trying to save messages for Deltas");
                            _logger.LogError("ERROR: {0} ", connection.Message);
                        }

                        Response gap = await Factory.FactoryDeltasControl.Gap(deltasControls, _options, databaserServer, _deltaService, _messageService, _logger);
                        if (gap.Result == "OK")
                        {
                            _logger.LogInformation("GAP: {0} Gap messages recorded", gap.Values.Where(x => x.Key == "TotalGapMessages").FirstOrDefault().Value);
                            //_logger.LogInformation("GAP: {0}", gap.Message);
                            //_logger.LogInformation("{0} is {1}", _options.WindowsTimeServiceName, gap.Values.Where(x => x.Key == "StopService").FirstOrDefault().Value);
                        }
                        else
                        {
                            _logger.LogError("GAP: Error trying to save messages for Deltas");
                            _logger.LogError("ERROR: {0} ", gap.Message);
                        }
                    }

                    // 4) Deltas de Eventos
                    var deltasEvents = deltas.Where(n => n.DeltaTypeId == 1).ToList();
                    if (deltasEvents.Count > 0)
                    {
                        Response saveDeltaEventMessages = await Factory.FactoryDeltasEvent.SaveDeltaEventMessages(deltasEvents, _deltaService, _messageService, _options, tskDelay, _logger);

                        if (saveDeltaEventMessages.Result == "OK")
                        {
                            _logger.LogInformation("EVENTS: {0} DeltaEvent messages recorded", saveDeltaEventMessages.Values.Where(x => x.Key == "TotalDeltasEvent").FirstOrDefault().Value);
                            //_logger.LogInformation("EVENTS: {0} is {1}", _options.WindowsTimeServiceName, saveDeltaEventMessages.Values.Where(x => x.Key == "StopService").FirstOrDefault().Value);
                        }
                        else
                        {
                            _logger.LogError("EVENTS: Error trying to save Deltas Events");
                            _logger.LogError("ERROR: {0} ", saveDeltaEventMessages.Message);
                        }
                    }

                    await Task.Delay(tskDelay, stoppingToken);
                }
            }
            catch (System.Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogCritical("{0} - FATAL ERROR: {1}", _options.LogInEventViewerName, ex.Message);
                _logger.LogCritical("{0} stopped unexpectedly", _options.LogInEventViewerName);
                _hostApplicationLifetime.StopApplication();
                throw;
            }
        }
    }
}

