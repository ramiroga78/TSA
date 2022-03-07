using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Utilities;

namespace TSA.NTPSyncService.Factory
{
    public static class FactoryNTPServersSync
    {
        public static async Task<Response> SyncServers(string routeKey,
                                                        IEnumerable<NTPServerDTO> databaseServers,
                                                        bool updateRegistry,
                                                        ILogger<Worker> logger)
        {
            Response response = new Response();
            try
            {
                //bool restartClockService = false;
                int countD = 0;
                int countR = 0;
                Regedit.SubKey = @routeKey;
                Response allKeyServers = Regedit.ReadAll();

                if (allKeyServers.Result == "OK")
                {
                    foreach (Response.ResponseValues server in allKeyServers.Values)
                    {
                        var databaseServer = databaseServers.Where(n => n.ServerUrl.Equals(server.Value)).Where(n => n.IsActive == true).FirstOrDefault();
                        if (databaseServer == null)
                        {
                            logger.LogWarning("SYNC: {0} Server doesn't exist in database and will be deleted from Registry", server.Value);

                            if (updateRegistry)
                            {
                                Response delete = new Response();

                                if(server.Key == "(Default)")
                                    delete = Regedit.DeleteKey("");
                                else
                                    delete = Regedit.DeleteKey(server.Key);

                                if (delete.Result == "OK")
                                    logger.LogInformation("SYNC: {0} was deleted from Registry", server.Value);
                                else
                                    logger.LogError("SYNC: {0}", delete.Message);
                            }
                            else
                            {
                                logger.LogWarning("SYNC: {0} was not deleted from Registry because the configuration doesn't allow it.", server.Value);
                            }
                        }
                        else
                        {
                            logger.LogInformation("SYNC: {0} server already exists in Registry", server.Value);
                        }
                    }
                }
                else
                {
                    logger.LogError("SYNC: {0}", allKeyServers.Message);
                }

                Response newAllKeyServers = Regedit.ReadAll();

                if (newAllKeyServers.Result == "OK")
                {
                    foreach (NTPServerDTO item in databaseServers)
                    {
                        if (!newAllKeyServers.Values.Any(x => x.Value == item.ServerUrl) && item.IsActive == true)
                        {
                            logger.LogWarning("SYNC: Server {0} doesn't exist in Registry and will be added", item.ServerUrl);

                            if (updateRegistry)
                            {
                                Response write = Regedit.Write((newAllKeyServers.Values.Count() + 1).ToString(), item.ServerUrl);

                                if (write.Result == "OK")
                                    logger.LogInformation("SYNC: Server {0} was added in Registry", item.ServerUrl);
                                else
                                    logger.LogError("SYNC: {0}", write.Message);
                            }
                            else
                            {
                                logger.LogWarning("SYNC: Server {0} was not added in Registry because the configuration doesn't allow it.", item.ServerUrl);
                            }
                        }
                    }
                }
                else
                {
                    logger.LogError("SYNC: {0}", newAllKeyServers.Message);
                }

                countD = databaseServers.Where(x => x.IsActive==true).Count();
                countR = newAllKeyServers.Values.Count();

                response.Values.Add(new Response.ResponseValues { Key = "TotalServersDatabase", Value = countD.ToString() });
                response.Values.Add(new Response.ResponseValues { Key = "TotalServersRegistry", Value = countR.ToString() });
                response.Result = "OK";
                response.Message = string.Empty;
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
