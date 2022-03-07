using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace TSA.Utilities
{
    public class EventLog
    {
        public static ICollection<EventRecord> GetEventsByTime(string route, int idEvent, uint milliseconds)
        {
            string queryString =
                "<QueryList>" +
                "  <Query Id='0' Path='" + route + "'>" +
                "    <Select Path='" + route + "'>*[System[(EventID=" + idEvent.ToString() + ") and TimeCreated[timediff(@SystemTime) &lt;= " + milliseconds.ToString() + "]]]</Select>" +
                "  </Query>" +
                "</QueryList>";
            EventLogQuery eventsQuery = new EventLogQuery("System", PathType.LogName, queryString);
            EventLogReader logReader = new EventLogReader(eventsQuery);
            EventRecord eventRecord;
            ICollection<EventRecord> eventRecords = null;
            while ((eventRecord = logReader.ReadEvent()) != null)
            {
                eventRecords.Add(eventRecord);
            }
            return eventRecords;
        }

        public static EventRecord GetOneEvent(string route, int idEvent, int time)
        {
            string queryString =
                "<QueryList>" +
                "  <Query Id='0' Path='" + route + "'>" +
                "    <Select Path='" + route + "'>*[System[(EventID=" + idEvent.ToString() + ") and TimeCreated[timediff(@SystemTime) &lt;= " + time.ToString() + "]]]</Select>" +
                "  </Query>" +
                "</QueryList>";
            EventLogQuery eventsQuery = new EventLogQuery("System", PathType.LogName, queryString);
            EventLogReader logReader = new EventLogReader(eventsQuery);
            EventRecord eventRecord;
            EventRecord retEventRecord = null;
            while ((eventRecord = logReader.ReadEvent()) != null)
            {
                retEventRecord = eventRecord;
                break;
            }
            return eventRecord;
        }
    }
}
