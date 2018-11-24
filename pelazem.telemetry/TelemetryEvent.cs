using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace pelazem.telemetry
{
	public class TelemetryEvent : ITelemetryEvent
	{
		#region Enums

		public enum EventTypes
		{
			NotSpecified = 0,
			Dependency,
			Event,
			Exception,
			Metric,
			Request,
			Trace,
			View
		}

		public enum Levels
		{
			NotSpecified = 0,
			Critical,
			Error,
			Information,
			Debug,
			Warning
		}

		#endregion

		#region Variables

		private IDictionary<string, string> _properties = null;

		private IDictionary<string, double> _metrics = null;

		#endregion

		#region Properties

		public EventTypes EventTypeValue { get; set; }

		public Levels LevelValue { get; set; }

		#endregion

		#region ITelemetryEvent

		public string EventType
		{
			get
			{
				return Enum.GetName(typeof(EventTypes), this.EventTypeValue);
			}
			set
			{
				Enum.TryParse(value, out EventTypes result);

				this.EventTypeValue = result;
			}
		}

		public string Level
		{
			get
			{
				return Enum.GetName(typeof(Levels), this.LevelValue);
			}
			set
			{
				Enum.TryParse(value, out Levels result);

				this.LevelValue = result;
			}
		}


		public string Id { get; set; }

		public string Name { get; set; }

		public string Message { get; set; }

		public string Target { get; set; }

		public string Version { get; set; }

		public string ResponseCode { get; set; }

		public string UserId { get; set; }

		public double? Value { get; set; } = null;

		public string Data { get; set; }

		public string Protocol { get; set; }

		public string Uri { get; set; }


		public DateTime? TimeStamp { get; set; } = DateTime.UtcNow;

		public TimeSpan? Duration { get; set; } = null;


		public IDictionary<string, string> Properties
		{
			get
			{
				if (_properties == null)
					_properties = new Dictionary<string, string>();

				return _properties;
			}
			set
			{
				_properties = value;
			}
		}

		public IDictionary<string, double> Metrics
		{
			get
			{
				if (_metrics == null)
					_metrics = new Dictionary<string, double>();

				return _metrics;
			}
			set
			{
				_metrics = value;
			}
		}


		public Exception Exception { get; set; }

		public string ProblemId { get; set; }


		public string Result { get; set; }

		public bool? Success { get; set; } = null;


		public string GetEventText()
		{
				string result = JsonConvert.SerializeObject(this as ITelemetryEvent);

				return result;
		}

		#endregion

		public override string ToString()
		{
			return GetEventText();
		}
	}
}
