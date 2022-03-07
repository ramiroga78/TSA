using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSA.NTPSyncService.Settings;
using TSA.Utilities;
using TSALibrary.Models;

namespace TSA.NTPSyncService.Factory
{
    public class FactoryDeltasControl
    {
        public static async Task<Response> Connection(List<DeltaDTO> deltasControls,
                                                            WorkerSettings options,
                                                            IEnumerable<NTPServerDTO> databaseServers,
                                                            IDeltaService deltaService,
                                                            IMessageService messageService,
                                                            ILogger<Worker> logger)
        {
            Response responseConnection = new Response();

            bool isConnected = false;

            try
            {
                foreach (DeltaDTO item in deltasControls)
                {
                    if (item.Id == options.DeltaConexionServidoresNTPId)
                    {
                        int count = 0;
                        List<Response.ResponseValues> rvalues = new List<Response.ResponseValues>();

                        NTPClient client;

                        foreach (NTPServerDTO server in databaseServers)
                        {
                            if (server.IsActive == true)
                            {
                                client = new NTPClient(server.ServerUrl);

                                try
                                {
                                    try
                                    {
                                        client.SyncConnect(false);

                                        isConnected = true;

                                        responseConnection.Result = "OK";
                                        //responseConnection.Message = "Connection to " + server.ServerUrl + " was successfully established.";

                                        logger.LogInformation("CONNECTION: Connection to {0} was successfully established.", server.ServerUrl);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        if (item.UserCount > 0)
                                        {
                                            IEnumerable<DeltaUserDTO> users = await deltaService.GetAllUsersByDeltaIdAndModelToDto(item.Id);

                                            foreach (DeltaUserDTO user in users)
                                            {
                                                if (user.IsActive == true)
                                                {
                                                    Message nTPDeltaMessage = new Message();

                                                    nTPDeltaMessage.IdType = item.DeltaTypeId;
                                                    nTPDeltaMessage.IdDelta = item.Id;
                                                    nTPDeltaMessage.IdUser = user.Id;
                                                    nTPDeltaMessage.Subject = item.ControlName;
                                                    nTPDeltaMessage.MessageBody = "Error al conectarse a " + server.ServerUrl;
                                                    nTPDeltaMessage.CreatedDate = DateTime.Now;
                                                    nTPDeltaMessage.Sent = false;
                                                    nTPDeltaMessage.SentError = false;
                                                    nTPDeltaMessage.ErrorReason = "";
                                                    nTPDeltaMessage.EditDate = null;

                                                    await messageService.AddAndSave(nTPDeltaMessage);
                                                    count++;
                                                }
                                            }

                                            responseConnection.Result = "OK";
                                            logger.LogError("CONNECTION: {0} FAILED: {1}", server.ServerUrl, ex.Message);
                                        }
                                        else
                                        {
                                            responseConnection.Result = "OK";
                                            //responseConnection.Message = "No users configured for Control Alert";
                                            logger.LogError("CONNECTION: No users configured for Control Alert");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    responseConnection.Result = "ERROR";
                                    responseConnection.Message = ex.Message;
                                }
                            }
                        }
                        Response.ResponseValues value = new Response.ResponseValues();

                        value.Key = "TotalControlMessages";
                        value.Value = count.ToString();
                        rvalues.Add(value);

                        responseConnection.Values = rvalues;

                        if (item.StopService == true && !isConnected)
                        {
                            Response stopService = Utilities.Service.StopService(options.WindowsTimeServiceName, 3000);
                            responseConnection.Values.Add(new Response.ResponseValues { Key = "StopService", Value = stopService.Message });
                            logger.LogInformation("CONNECTION: Service was stopped: {0}", stopService.Message);
                        }
                        else
                        {
                            Response serviceStatus = Utilities.Service.GetServiceStatus(options.WindowsTimeServiceName);
                            responseConnection.Values.Add(new Response.ResponseValues { Key = "StopService", Value = serviceStatus.Message });
                            logger.LogInformation("CONNECTION: Service status: {0}", serviceStatus.Message);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                responseConnection.Result = "ERROR";
                responseConnection.Message = ex.Message;
            }

            return responseConnection;
        }

        public static async Task<Response> Gap(List<DeltaDTO> deltasControls,
                                                            WorkerSettings options,
                                                            IEnumerable<NTPServerDTO> databaseServers,
                                                            IDeltaService deltaService,
                                                            IMessageService messageService,
                                                            ILogger<Worker> logger)
        {
            Response responseGap = new Response();

            bool timeOffset = false;
            bool isConnected = false;

            try
            {
                foreach (DeltaDTO item in deltasControls)
                {
                    if (item.Id == options.DeltaGapServidorNTPyWindowsId)
                    {
                        int count = 0;
                        List<Response.ResponseValues> rvalues = new List<Response.ResponseValues>();

                        NTPClient client;
                        foreach (NTPServerDTO server in databaseServers.Where(x => x.IsActive == true))
                        {
                            //if (server.IsActive == true)
                            //{
                            client = new NTPClient(server.ServerUrl);

                            try
                            {
                                client.SyncConnect(false);

                                isConnected = true;

                                switch (item.ControlOperator)
                                {
                                    case ">":
                                        timeOffset = client.LocalClockOffset > Convert.ToInt32(item.ControlOperatorValue);
                                        break;
                                    case ">=":
                                        timeOffset = client.LocalClockOffset >= Convert.ToInt32(item.ControlOperatorValue);
                                        break;
                                    case "<":
                                        timeOffset = client.LocalClockOffset < Convert.ToInt32(item.ControlOperatorValue);
                                        break;
                                    case "<=":
                                        timeOffset = client.LocalClockOffset <= Convert.ToInt32(item.ControlOperatorValue);
                                        break;
                                    case "=":
                                        timeOffset = client.LocalClockOffset == Convert.ToInt32(item.ControlOperatorValue);
                                        break;
                                }

                                if (item.UserCount > 0)
                                {
                                    if (timeOffset)
                                    {
                                        IEnumerable<DeltaUserDTO> users = await deltaService.GetAllUsersByDeltaIdAndModelToDto(item.Id);
                                        foreach (DeltaUserDTO user in users)
                                        {
                                            if (user.IsActive == true)
                                            {
                                                Message nTPDeltaMessage = new Message();

                                                nTPDeltaMessage.IdType = item.DeltaTypeId;
                                                nTPDeltaMessage.IdDelta = item.Id;
                                                nTPDeltaMessage.IdUser = user.Id;
                                                nTPDeltaMessage.Subject = item.ControlName;
                                                nTPDeltaMessage.MessageBody = String.Format("GAP en {0} {1} {2}", server.ServerUrl, item.ControlOperator, item.ControlOperatorValue);
                                                nTPDeltaMessage.CreatedDate = DateTime.Now;
                                                nTPDeltaMessage.Sent = false;
                                                nTPDeltaMessage.SentError = false;
                                                nTPDeltaMessage.ErrorReason = "";
                                                nTPDeltaMessage.EditDate = null;

                                                await messageService.AddAndSave(nTPDeltaMessage);
                                                count++;
                                            }
                                        }
                                        //responseGap.Message = "Server time offset is " + client.LocalClockOffset;
                                        logger.LogWarning("GAP: Server time offset with {0} is {1}",server.ServerUrl, client.LocalClockOffset);
                                    }
                                    else
                                        //responseGap.Message = "TimeOffset is OK";
                                        logger.LogInformation("GAP: Time offset is OK with {0}", server.ServerUrl);
                                }
                                else
                                {
                                    if (timeOffset)
                                        //responseGap.Message = "Server time offset is " + client.LocalClockOffset + " but no users were configured for Gap Alert";
                                        logger.LogWarning("GAP: Server time offset with {0} is {1} but no users were configured for Gap Alert", server.ServerUrl, client.LocalClockOffset);
                                    else
                                        //responseGap.Message = "TimeOffset is OK but no users were configured for Gap Alert";
                                        logger.LogWarning("GAP: Server time offset is OK with {0} but no users were configured for Gap Alert", server.ServerUrl);
                                }
                                responseGap.Result = "OK";
                            }
                            catch (Exception ex)
                            {
                                responseGap.Result = "OK";
                                //responseGap.Message = server.ServerUrl + " failed: " + ex.Message;
                                logger.LogWarning("GAP: {0} FAILED: {1}", server.ServerUrl, ex.Message);
                            }
                            //}
                        }
                        Response.ResponseValues value = new Response.ResponseValues();

                        value.Key = "TotalGapMessages";
                        value.Value = count.ToString();
                        rvalues.Add(value);

                        responseGap.Values = rvalues;

                        if (item.StopService == true && timeOffset)
                        {
                            Response stopService = Utilities.Service.StopService(options.WindowsTimeServiceName, 3000);

                            responseGap.Values.Add(new Response.ResponseValues { Key = "StopService", Value = "Service was stopped: " + stopService.Message });
                            logger.LogInformation("GAP: Service was stopped: {0}", stopService.Message);
                        }
                        else
                        {
                            Response serviceStatus = Utilities.Service.GetServiceStatus(options.WindowsTimeServiceName);

                            responseGap.Values.Add(new Response.ResponseValues { Key = "StopService", Value = serviceStatus.Message });
                            logger.LogInformation("CONNECTION: Service status: {0}", serviceStatus.Message);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                responseGap.Result = "ERROR";
                responseGap.Message = ex.Message;
            }

            return responseGap;
        }
    }
}
