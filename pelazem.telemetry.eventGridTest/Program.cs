using System;
using System.Collections.Generic;
using System.Linq;
using pelazem.telemetry;
using pelazem.telemetry.eventGridSink;

namespace pelazem.telemetry.eventGridTest
{
	class Program
	{
		const string APPSETTINGSPATH = "appsettings.json";

		static void Main(string[] args)
		{
			Publish();

			Console.WriteLine("Press any key when ready...");
			Console.Read();
		}

		static void Publish()
		{
			AppConfig config = AppConfig.GetInstance(APPSETTINGSPATH);

			List<TelemetryEvent> events = new List<TelemetryEvent>();

			for (int i = 1; i <= 10; i++)
			{
				TelemetryEvent te = new TelemetryEvent();
				te.Duration = new TimeSpan(0, 3, 5);
				te.EventTypeValue = TelemetryEvent.EventTypes.Event;
				te.Id = Guid.NewGuid().ToString();
				te.LevelValue = TelemetryEvent.Levels.Information;
				te.Message = "Message " + i.ToString();
				te.Metrics.Add("FooMetric", i);
				te.Value = i;
				te.Name = "Event " + i.ToString();
				te.ProblemId = "Problem " + i.ToString();
				te.Properties.Add("FooProperty", i.ToString());
				te.ResponseCode = "200";
				te.Result = "Success";
				te.Success = true;
				te.Target = "FooTarget";
				te.TimeStamp = DateTime.UtcNow;
				te.Uri = "http://portal.azure.com";
				te.UserId = "pelazem";
				te.Version = "1.0";

				events.Add(te);
			}

			EventGridTelemetrySink sink = new EventGridTelemetrySink(config.EventGrid.TopicHostName, config.EventGrid.TopicKey);

			sink.Send(events);
		}
	}
}
