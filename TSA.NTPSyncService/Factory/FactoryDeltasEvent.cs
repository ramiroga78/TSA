using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSA.NTPSyncService.Settings;
using TSA.Utilities;
using TSALibrary.Models;

namespace TSA.NTPSyncService.Factory
{
    public class FactoryDeltasEvent
    {
        public static async Task<Response> SaveDeltaEventMessages(List<DeltaDTO> deltasEvents,
                                                                  IDeltaService deltaService,
                                                                  IMessageService messageService,
                                                                  WorkerSettings options,
                                                                  int time,
                                                                  ILogger<Worker> logger)
        {
            int count = 0;

            Response response = new Response();
            List<Response.ResponseValues> rvalues = new List<Response.ResponseValues>();

            foreach (DeltaDTO item in deltasEvents)
            {
                try
                {
                    var Event = Utilities.EventLog.GetOneEvent(options.WindowsTimeService, Convert.ToInt32(item.EventCode), time);

                    if (Event != null)
                    {
                        try
                        {
                            string messageBody = string.Empty;

                            messageBody = "TimeCreated : " + Event.TimeCreated;
                            messageBody += "Id : " + Event.Id;
                            messageBody += "ProcessId : " + Event.ProcessId;
                            messageBody += "=================================================";
                            messageBody += Event.FormatDescription();
                            messageBody += "=================================================";
                            messageBody += "=================================================";

                            IEnumerable<DeltaUserDTO> users = await deltaService.GetAllUsersByDeltaIdAndModelToDto(item.Id);

                            foreach (DeltaUserDTO user in users)
                            {
                                if (user.IsActive == true)
                                {
                                    Message deltaEventMessage = new Message();

                                    deltaEventMessage.IdType = item.DeltaTypeId;
                                    deltaEventMessage.IdDelta = item.Id;
                                    deltaEventMessage.IdUser = user.Id;
                                    deltaEventMessage.Subject = item.EventCode + " " + item.EventName;
                                    deltaEventMessage.MessageBody = messageBody;
                                    deltaEventMessage.CreatedDate = DateTime.Now;
                                    deltaEventMessage.Sent = false;
                                    deltaEventMessage.SentError = false;
                                    deltaEventMessage.ErrorReason = "";
                                    deltaEventMessage.EditDate = null;

                                    await messageService.AddAndSave(deltaEventMessage);
                                    count++;
                                }
                            }

                            Response.ResponseValues value = new Response.ResponseValues();

                            value.Key = "TotalDeltasEvent";
                            value.Value = count.ToString();
                            rvalues.Add(value);

                            response.Result = "OK";
                            response.Message = string.Empty;
                            response.Values = rvalues;
                        }
                        catch (Exception ex)
                        {
                            response.Result = "ERROR";
                            response.Message = ex.Message;
                        }

                        if (item.StopService == true)
                        {
                            Response stopService = Utilities.Service.StopService(options.WindowsTimeServiceName, 3000);

                            response.Values.Add(new Response.ResponseValues { Key = "StopService", Value = "Service was stopped: " + stopService.Message });
                            logger.LogInformation("EVENTS: Service was stopped: {0}", stopService.Message);
                        }
                        else
                        {
                            Response serviceStatus = Utilities.Service.GetServiceStatus(options.WindowsTimeServiceName);

                            response.Values.Add(new Response.ResponseValues { Key = "StopService", Value = serviceStatus.Message });
                            logger.LogInformation("EVENTS: Service status: {0}", serviceStatus.Message);
                        }
                    }
                    else
                    {
                        response.Result = "OK";
                        response.Message = "No events found.";
                        response.Values.Add(new Response.ResponseValues { Key = "TotalDeltasEvent", Value = "0" });
                    }
                }
                catch (Exception ex)
                {
                    response.Result = "ERROR";
                    response.Message = ex.Message;
                }
            }
            return response;
        }
    }
}
