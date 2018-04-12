using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using pelazem.telemetry;
using pelazem.telemetry.appInsightsSink;

namespace pelazem.telemetry.appInsightsTest
{
	class Program
	{
		const string APPSETTINGSPATH = "appsettings.json";
		const string APPINSIGHTSINSTRUMENTATIONKEY = "AppInsightsInstrumentationKey";

		static void Main(string[] args)
		{
			string appInsightsInstrumentationKey = GetAppInsightsInstrumentationKey();

			if (string.IsNullOrWhiteSpace(appInsightsInstrumentationKey))
				throw new Exception("No App Insights Instrumentation Key could be retrieved from configuration!");

			AppInsightsTelemetrySink sink = new AppInsightsTelemetrySink(appInsightsInstrumentationKey);

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

			sink.Send(events);

			sink.Client.Flush();
		}

		private static string GetAppInsightsInstrumentationKey()
		{
			var config = GetConfig();

			return config[APPINSIGHTSINSTRUMENTATIONKEY];
		}

		private static IConfigurationRoot GetConfig()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile(APPSETTINGSPATH, false, false)
			;

			return builder.Build();
		}
	}

	public class AppConfig
	{
		public AppInsights AppInsights { get; set; }
	}

	public class AppInsights
	{
		public string InstrumentationKey { get; set; }
	}
}
