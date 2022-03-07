using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TSA.Utilities
{
    public class Service
    {
        public static Response GetServiceStatus(string serviceName)
        {
            Response response = new Response();

            try
            {
                ServiceController sc = new(serviceName);

                sc.Refresh();

                response.Result = "OK";
                response.Message = sc.Status.ToString();
            }
            catch (System.Exception ex)
            {
                response.Result = "ERROR";
                response.Message = ex.Message;
            }

            return response;
        }

        public static Response StartService(string serviceName, int timeOut)
        {
            Response response = new Response();

            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeOut);

                ServiceController sc = new(serviceName);

                if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) ||
                    (sc.Status.Equals(ServiceControllerStatus.StopPending)) ||
                    (sc.Status.Equals(ServiceControllerStatus.Paused)) ||
                    (sc.Status.Equals(ServiceControllerStatus.PausePending)))
                {
                    // Start the service if the current status is stopped or paused.
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }

                sc.Refresh();

                response.Result = "OK";
                response.Message = "Service Running";

            }
            catch (System.Exception ex)
            {
                response.Result = "ERROR";
                response.Message = ex.Message;
            }

            return response;
        }

        public static Response StopService(string serviceName, int timeOut)
        {
            Response response = new Response();

            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeOut);

                ServiceController sc = new(serviceName);

                if ((sc.Status.Equals(ServiceControllerStatus.Running)) ||
                    (sc.Status.Equals(ServiceControllerStatus.StartPending)) ||
                    (sc.Status.Equals(ServiceControllerStatus.Paused)) ||
                    (sc.Status.Equals(ServiceControllerStatus.PausePending)))
                {
                    // Stop the service if the current status is running or paused.
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }

                sc.Refresh();

                response.Result = "OK";
                response.Message = "Service Stopped";
            }
            catch (System.Exception ex)
            {
                response.Result = "ERROR";
                response.Message = ex.Message;
            }

            return response;
        }

        public static Response RestartService(string serviceName, int timeOut)
        {
            Response response = new Response();

            try
            {
                ServiceController sc = new(serviceName);

                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeOut);

                if ((sc.Status.Equals(ServiceControllerStatus.Running)) ||
                    (sc.Status.Equals(ServiceControllerStatus.StartPending)) ||
                    (sc.Status.Equals(ServiceControllerStatus.Paused)) ||
                    (sc.Status.Equals(ServiceControllerStatus.PausePending)))
                {
                    // Stop the service if the current status is running or paused.
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeOut - (millisec2 - millisec1));

                //Start the service
                sc.Start();
                sc.WaitForStatus(ServiceControllerStatus.Running, timeout);

                sc.Refresh();

                response.Result = "OK";
                response.Message = "Service Restarted";
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
