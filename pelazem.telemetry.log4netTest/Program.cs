using System;
using pelazem.telemetry;
using pelazem.telemetry.log4netSink;

namespace pelazem.telemetry.log4netTest
{
	class Program
	{
		static void Main(string[] args)
		{
			TelemetrySink sink = new TelemetrySink();

			for (int i = 1; i <= 1000; i++)
			{
				sink.TrackEvent
				(
					"Test Event",
					"Test Message " + i.ToString(),
					"Test Result " + i.ToString(),
					Telemetry.Level.Information,
					"pelazem",
					DateTime.UtcNow,
					null,
					null,
					null,
					null
				);
			}

			System.Threading.Thread.Sleep(1000);
		}
	}
}
