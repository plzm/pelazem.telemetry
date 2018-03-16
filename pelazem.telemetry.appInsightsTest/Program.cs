using System;
using pelazem.telemetry;
using pelazem.telemetry.appInsightsSink;

namespace pelazem.telemetry.appInsightsTest
{
	class Program
	{
		static void Main(string[] args)
		{
			string name = Guid.NewGuid().ToString();

			TelemetrySink sink = new TelemetrySink("c836de01-85a7-4810-b8e5-76066b12822f");

			for (int i = 1; i <= 300; i++)
			{
				sink.TrackEvent
				(
					name,
					"Test Message " + i.ToString(),
					"Test Result " + i.ToString(),
					Telemetry.Level.Debug,
					"pelazem",
					DateTime.UtcNow,
					null,
					null,
					null,
					null
				);
			}

			sink.Client.Flush();
		}
	}
}
