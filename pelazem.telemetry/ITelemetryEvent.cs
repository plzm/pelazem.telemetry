using System;
using System.Collections.Generic;

namespace pelazem.telemetry
{
	public interface ITelemetryEvent
	{
		string EventType { get; set; }
		string Level { get; set; }

		string Id { get; set; }
		string Name { get; set; }
		string Message { get; set; }
		string Target { get; set; }
		string Version { get; set; }
		string ResponseCode { get; set; }
		string UserId { get; set; }
		double? Value { get; set; }
		string Data { get; set; }
		string Protocol { get; set; }
		string Uri { get; set; }

		DateTime? TimeStamp { get; set; }
		TimeSpan? Duration { get; set; }

		IDictionary<string, string> Properties { get; set; }
		IDictionary<string, double> Metrics { get; set; }

		Exception Exception { get; set; }
		string ProblemId { get; set; }

		string Result { get; set; }
		bool? Success { get; set; }

		/// <summary>
		/// String summary of all event information, for sinks that mainly accept a structured text field
		/// </summary>
		/// <returns></returns>
		string GetEventText();
	}
}