using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSA.MessagingService.Settings;
using TSA.Utilities;
using TSALibrary.Models;

namespace TSA.MessagingService
{
    public class Worker : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger<Worker> _logger;
        private readonly WorkerSettings _options;
        private readonly IMessageService _messageService;
        private readonly IAlertService _alertService;
        private readonly IUserService _userService;

        public Worker(IHostApplicationLifetime hostApplicationLifetime,
            ILogger<Worker> logger,
            WorkerSettings options,
            IAlertService alertService,
            IMessageService messageService,
            IUserService userService)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
            _options = options;
            _alertService = alertService;
            _messageService = messageService;
            _userService = userService;
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

            Email.ServerSettings serverSettings = new Email.ServerSettings();

            serverSettings.Host = _options.SMTPHost;
            serverSettings.Port = _options.SMTPPort;
            serverSettings.AuthenticationEmail = _options.AuthenticationEmail;
            serverSettings.AuthenticationPassword = _options.AuthenticationPassword;

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {

                    _logger.LogInformation("{0} running at: {time}", _options.LogInEventViewerName, DateTimeOffset.Now);

                    Response nTPServiceResponse = new Response();
                    Response timeServiceResponse = new Response();
                    Response emailResponse = new Response();

                    // 1) Leer estado NTP Service
                    nTPServiceResponse = Service.GetServiceStatus(_options.NTPServiceName);

                    if (nTPServiceResponse.Result == "OK")
                    {
                        if (nTPServiceResponse.Message.Equals(ServiceControllerStatus.Stopped.ToString()))
                        {
                            _logger.LogError("{0} is not running", _options.NTPServiceName);
                            _logger.LogError("{0}: {1} - {2}", _options.NTPServiceName, nTPServiceResponse.Result, nTPServiceResponse.Message);

                            Response saveMessage = await Factory.FactoryMessaging.SaveMessageForServices(_alertService, _messageService,
                                _options.NTPServiceAlertId, _options.NTPServiceName, 2);

                            if (saveMessage.Result == "OK")
                            {
                                _logger.LogInformation("SaveMessageForServices: {0} Messages saved", saveMessage.Values.Where(x => x.Key == "TotalMessages").FirstOrDefault().Value);
                            }
                            else
                            {
                                _logger.LogError("SaveMessageForServices: Error trying to save Messages for service " + _options.NTPServiceName);
                                _logger.LogError("ERROR: {0} ", saveMessage.Message);
                            }
                        }
                        else
                        {
                            _logger.LogInformation("{0} is running", _options.NTPServiceName);
                        }
                    }
                    else
                    {
                        _logger.LogError("ERROR: {0} ", nTPServiceResponse.Message);
                    }


                    // 2) Leer estado Time Service
                    timeServiceResponse = Service.GetServiceStatus(_options.timeServiceName);

                    if (timeServiceResponse.Result == "OK")
                    {
                        if (timeServiceResponse.Message.Equals(ServiceControllerStatus.Stopped.ToString()))
                        {
                            _logger.LogError("{0} is not running", _options.timeServiceName);
                            _logger.LogError("{0}: {1} - {2}", _options.timeServiceName, timeServiceResponse.Result, timeServiceResponse.Message);

                            Response saveMessage = await Factory.FactoryMessaging.SaveMessageForServices(_alertService, _messageService,
                                _options.TimeServiceAlertId, _options.timeServiceName, 2);

                            if (saveMessage.Result == "OK")
                            {
                                _logger.LogInformation("SaveMessageForServices: {0} Messages saved", saveMessage.Values.Where(x => x.Key == "TotalMessages").FirstOrDefault().Value);
                            }
                            else
                            {
                                _logger.LogError("SaveMessageForServices: Error trying to save Messages for service " + _options.timeServiceName);
                                _logger.LogError("ERROR: {0} ", saveMessage.Message);
                            }
                        }
                        else
                        {
                            _logger.LogInformation("{0} is running", _options.timeServiceName);
                        }
                    }
                    else
                    {
                        _logger.LogError("ERROR: {0} ", timeServiceResponse.Message);
                    }

                    // 3) Enviar mensajes
                    IEnumerable<Message> messages = await _messageService.GetTopNMessages(_options.MessagesNumber, "CreatedDate");

                    foreach (Message message in messages)
                    {
                        Email email = new Email();

                        User user = await _userService.GetUserById(message.IdUser);

                        email.From = _options.AuthenticationEmail;
                        email.To = user.Email;
                        email.Subject = message.Subject;
                        email.Body = message.MessageBody;
                        email.Settings = serverSettings;

                        emailResponse = await email.SendEmailAsync(email);

                        if (emailResponse.Result == "OK")
                        {
                            _logger.LogInformation("Email {0} sent To {1}", email.Subject, email.To);

                            await _messageService.MarkAsSent(message.Id, user.Email);
                        }
                        else
                        {
                            _logger.LogError("SendEmailServices: Error trying to send email {0} To {1}", email.Subject, email.To);
                            _logger.LogError("ERROR: {0} ", emailResponse.Message);

                            await _messageService.WriteSendError(message.Id, emailResponse.Message);
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
