using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSA.Utilities;
using TSALibrary.Models;

namespace TSA.MessagingService.Factory
{
    public static class FactoryMessaging
    {
        public static async Task<Response> SaveMessageForServices(
                IAlertService alertService,
                IMessageService messageService,
                int alertId, string serviceName, int typeId)
        {
            Response response = new Response();
            List<Response.ResponseValues> rvalues = new List<Response.ResponseValues>();

            int count = 0;

            try
            {
                AlertDTO alert = await alertService.GetAlertById(Convert.ToInt32(alertId));

                if (alert != null)
                {
                    IEnumerable<AlertUserDTO> users = await alertService.GetAllUsersByAlertIdAndModelToDto(alert.Id);

                    foreach (AlertUserDTO user in users)
                    {
                        if (user.IsActive == true)
                        {
                            Message serviceMessage = new Message();

                            serviceMessage.IdType = typeId;
                            serviceMessage.IdAlerta = alert.Id;
                            serviceMessage.IdUser = user.UserId;
                            serviceMessage.SentTo = "";
                            serviceMessage.Subject = alert.Name;
                            serviceMessage.MessageBody = "El servicio " + serviceName + " se ha detenido.";
                            serviceMessage.CreatedDate = DateTime.Now;
                            serviceMessage.Sent = false;
                            serviceMessage.SentError = false;
                            serviceMessage.ErrorReason = "";
                            serviceMessage.EditDate = null;

                            await messageService.AddAndSave(serviceMessage);
                            count++;
                        }
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