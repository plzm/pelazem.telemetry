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

			TelemetrySink sink = new TelemetrySink(APP INSIGHTS INSTRUMENTATION KEY);

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
