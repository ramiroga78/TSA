using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSA.NTPSyncService.Settings
{
    public class WorkerSettings
    {
        public string TSAMessagingService { get; set; }
        public int TSAMessagingServiceAlertId { get; set; }
        public string TimerLoopInSeconds { get; set; }
        public int LogInEventViewer { get; set; }
        public string LogInEventViewerName { get; set; }
        public string NTPServersRegistrySubKey { get; set; }
        public bool UpdateRegistry { get; set; }
        public int DeltaGapServidorNTPyWindowsId { get; set; }
        public int DeltaConexionServidoresNTPId { get; set; }
        public string WindowsTimeService { get; set; }
        public string WindowsTimeServiceName { get; set; }
    }
}
