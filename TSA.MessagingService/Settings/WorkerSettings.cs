using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSA.MessagingService.Settings
{
    public class WorkerSettings
    {
        public string TimerLoopInSeconds { get; set; }
        public string LogInEventViewerName { get; set; }
        public string LogInEventViewer { get; set; }
        public int MessagesNumber { get; set; }
        public int NTPServiceAlertId { get; set; }
        public int TimeServiceAlertId { get; set; }
        public string NTPServiceName { get; set; }
        public string timeServiceName { get; set; }
        public string SMTPHost { get; set; }
        public int SMTPPort { get; set; }
        public string AuthenticationEmail { get; set; }
        public string AuthenticationPassword { get; set; }
    }
}
