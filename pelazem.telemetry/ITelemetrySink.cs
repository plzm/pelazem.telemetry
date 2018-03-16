using System;
using System.Collections.Generic;

namespace pelazem.telemetry
{
	public interface ITelemetrySink
	{
		string Name { get; }

		void TrackDependency(string type, string name, string message, string target, string result, string userId, DateTime timeStamp, TimeSpan? duration = null, bool? success = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);

		void TrackEvent(string name, string message, string result, Telemetry.Level level, string userId, DateTime timeStamp, TimeSpan? duration = null, bool? success = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);

		void TrackException(Exception exception, string message, string problemId, string userId, DateTime timeStamp, TimeSpan? duration = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);

		void TrackMetric(string metricName, double metricValue, string userId, DateTime timeStamp);

		void TrackRequest(string name, string message, string responseCode, string userId, DateTime timeStamp, TimeSpan? duration = null, bool? success = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);

		void TrackTrace(string message, Telemetry.Level level, string userId, DateTime timeStamp, TimeSpan? duration = null, IDictionary<string, string> properties = null);

		void TrackView(string name, string message, Uri url, string userId, DateTime timeStamp, TimeSpan? duration = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null);
	}
}
