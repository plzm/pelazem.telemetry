using System;
using System.Collections.Generic;
using System.Linq;
using pelazem.telemetry;
using pelazem.telemetry.log4netSink;

namespace pelazem.telemetry.log4netTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Log4NetTelemetrySink sink = new Log4NetTelemetrySink();

			List<TelemetryEvent> events = new List<TelemetryEvent>();

			for (int i = 1; i <= 1000; i++)
			{
				TelemetryEvent te = new TelemetryEvent();

				te.Data = "Data " + i.ToString();
				te.Duration = new TimeSpan(i, 0, 0);
				te.EventTypeValue = TelemetryEvent.EventTypes.Trace;
				te.Id = i.ToString();
				te.LevelValue = TelemetryEvent.Levels.Debug;
				te.Message = "Message " + i.ToString();
				te.Metrics.Add("m", i);
				te.Name = "Name " + i.ToString();
				te.ProblemId = "Problem " + i.ToString();
				te.Properties.Add("p", i.ToString());
				te.Protocol = "TCP";
				te.ResponseCode = "OK";
				te.Result = "The Result";
				te.Success = true;
				te.Target = "The Tarjay!";
				te.TimeStamp = DateTime.UtcNow;
				te.Uri = "http://portal.azure.com";
				te.UserId = "U" + i.ToString();
				te.Value = i;
				te.Version = i.ToString();

				events.Add(te);
			}

			sink.Send(events);

			System.Threading.Thread.Sleep(2000);
		}
	}
}
