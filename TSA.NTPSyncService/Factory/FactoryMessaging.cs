using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSA.Utilities;
using TSALibrary.Models;

namespace TSA.NTPSyncService.Factory
{
    public static class FactoryMessaging
    {
        public static async Task<Response> SaveMessageForServices(IAlertService alertService, 
                                                                  IMessageService messageService,
                                                                  int alertId, 
                                                                  string serviceName, 
                                                                  int typeId ) {
            Response response = new Response();
            List<Response.ResponseValues> rvalues = new List<Response.ResponseValues>();

            int count = 0;
            try
            {
                AlertDTO nTPAlert = await alertService.GetAlertById(Convert.ToInt32(alertId));
                if (nTPAlert != null)
                {
                    IEnumerable<AlertUserDTO> users = await alertService.GetAllUsersByAlertIdAndModelToDto(nTPAlert.Id);
                    foreach (AlertUserDTO user in users)
                    {
                        Message nTPAlertMessage = new Message();
                        nTPAlertMessage.IdType = typeId;
                        nTPAlertMessage.IdAlerta = nTPAlert.Id;
                        nTPAlertMessage.IdUser = user.UserId;
                        nTPAlertMessage.Subject = nTPAlert.Name;
                        nTPAlertMessage.MessageBody = "El servicio " + serviceName + " se ha detenido.";
                        nTPAlertMessage.CreatedDate = DateTime.Now;
                        nTPAlertMessage.Sent = false;
                        nTPAlertMessage.SentError = false;
                        nTPAlertMessage.ErrorReason = "";
                        nTPAlertMessage.EditDate = null;

                        await messageService.AddAndSave(nTPAlertMessage);
                        count++;
                    }
                }
                Response.ResponseValues value = new Response.ResponseValues();

                value.Key = "TotalMessages";
                value.Value = count.ToString();
                rvalues.Add(value);

                response.Result = "OK";
                response.Message = string.Empty;
                response.Values = rvalues;
            }
            catch (System.Exception ex)
            {
                response.Result = "ERROR";
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
