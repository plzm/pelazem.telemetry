using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace pelazem.telemetry.eventGridTest
{
	public class AppConfig
	{
		public static AppConfig GetInstance(string appSettingsJsonFilePath)
		{
				var builder = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile(appSettingsJsonFilePath, false, false)
				;

				return builder.Build().Get<AppConfig>();
		}

		public EventGrid EventGrid { get; set; }
	}

	public class EventGrid
	{
		public string TopicHostName { get; set; }
		public string TopicKey { get; set; }
	}
}
