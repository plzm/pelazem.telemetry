using System;
using System.Collections.Generic;

namespace pelazem.telemetry
{
	public interface ITelemetrySink
	{
		void Send(ITelemetryEvent telemetryEvent);

		void Send(IEnumerable<ITelemetryEvent> telemetryEvents);
	}
}
