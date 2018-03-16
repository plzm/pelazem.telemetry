using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pelazem.util;

namespace pelazem.telemetry.log4netSink
{
	public class LogEvent
	{
		#region Constants

		private const string EQ = "=";
		private const string DELIM = "::";

		#endregion

		#region Variables

		private Dictionary<string, string> _properties = null;
		private Dictionary<string, double> _metrics = null;

		#endregion

		#region Properties

		public string EventType { get; set; }

		public string Name { get; set; }

		public string Message { get; set; }

		public Uri Uri { get; set; }

		public string UserId { get; set; }

		public DateTime TimeStamp { get; set; }

		public TimeSpan? Duration { get; set; }

		public string Result { get; set; }

		public bool? Success { get; set; }

		public IDictionary<string, string> Properties
		{
			get
			{
				if (_properties == null)
					_properties = new Dictionary<string, string>();

				return _properties;
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
		}

		#endregion

		public override string ToString()
		{
			return
				nameof(EventType) + EQ + this.EventType + DELIM +
				(!string.IsNullOrWhiteSpace(this.Name) ? nameof(this.Name) + EQ + this.Name + DELIM : string.Empty) +
				(!string.IsNullOrWhiteSpace(this.Message) ? nameof(this.Message) + EQ + this.Message + DELIM : string.Empty) +
				(this.Uri != null && !string.IsNullOrWhiteSpace(this.Uri.AbsoluteUri) ? nameof(this.Uri) + EQ + this.Uri.AbsoluteUri + DELIM : string.Empty) +
				(!string.IsNullOrWhiteSpace(this.UserId) ? nameof(this.UserId) + EQ + this.UserId + DELIM : string.Empty) +
				(this.TimeStamp != null && this.TimeStamp > DateTime.MinValue ? nameof(this.TimeStamp) + EQ + string.Format("{0:O}", this.TimeStamp) + DELIM : string.Empty) +
				(this.Duration != null ? nameof(this.Duration) + EQ + string.Format("{0:g}", this.Duration) + DELIM : string.Empty) +
				(!string.IsNullOrWhiteSpace(this.Result) ? nameof(this.Result) + EQ + this.Result + DELIM : string.Empty) +
				(this.Success != null ? nameof(this.Success) + EQ + this.Success.ToString() + DELIM : string.Empty) +
				(_properties != null ? _properties.GetDelimitedList(DELIM, string.Empty, false) : string.Empty) +
				(_metrics != null ? _metrics.GetDelimitedList(DELIM, string.Empty, false) : string.Empty)
			;
		}
	}
}
