using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pelazem.telemetry
{
	public class Telemetry : ITelemetrySink
	{
		#region Enums

		public enum Level
		{
			Critical,
			Error,
			Information,
			Debug,
			Warning
		}

		#endregion

		#region Variables

		#endregion

		#region Properties

		private List<ITelemetrySink> TelemetrySinksList { get; set; } = new List<ITelemetrySink>();

		public IEnumerable<ITelemetrySink> TelemetrySinks
		{
			get { return this.TelemetrySinksList.AsEnumerable(); }
		}

		#endregion

		#region Constructors

		private Telemetry() { }

		public Telemetry(ITelemetrySink telemetrySink)
		{
			if (telemetrySink == null)
				throw new ArgumentNullException();

			this.TelemetrySinksList.Add(telemetrySink);
		}

		public Telemetry(IEnumerable<ITelemetrySink> telemetrySinks)
		{
			if (telemetrySinks == null)
				throw new ArgumentNullException();

			if (telemetrySinks.Count() == 0)
				throw new ArgumentException(Properties.Resources.TelemetryListEmpty);

			if (!telemetrySinks.Any(s => s != null))
				throw new ArgumentException(Properties.Resources.TelemetryListNoValidSinks);

			this.TelemetrySinksList.AddRange(telemetrySinks.Where(ts => ts != null));
		}

		#endregion

		#region Methods

		private void Track(IEnumerable<Action> actions)
		{
			try
			{
				Task.WaitAll(actions.Select(a => Task.Factory.StartNew(a)).ToArray());
			}
			catch (AggregateException ae)
			{
				throw ae;
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		#endregion

		#region ITelemetrySink

		public string Name { get { return nameof(Telemetry); } }

		public void TrackDependency(string type, string name, string message, string target, string result, string userId, DateTime timeStamp, TimeSpan? duration = null, bool? success = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
		{
			Track(this.TelemetrySinksList.Select((sink) => new Action(() => sink.TrackDependency(type, name, message, target, result, userId, timeStamp, duration, success, properties, metrics))));
		}

		public void TrackEvent(string name, string message, string result, Telemetry.Level level, string userId, DateTime timeStamp, TimeSpan? duration = null, bool? success = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
		{
			Track(this.TelemetrySinksList.Select((sink) => new Action(() => sink.TrackEvent(name, message, result, level, userId, timeStamp, duration, success, properties, metrics))));
		}

		public void TrackException(Exception exception, string message, string problemId, string userId, DateTime timeStamp, TimeSpan? duration = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
		{
			Track(this.TelemetrySinksList.Select((sink) => new Action(() => sink.TrackException(exception, message, problemId, userId, timeStamp, duration, properties, metrics))));
		}

		public void TrackMetric(string metricName, double metricValue, string userId, DateTime timeStamp)
		{
			Track(this.TelemetrySinksList.Select((sink) => new Action(() => sink.TrackMetric(metricName, metricValue, userId, timeStamp))));
		}

		public void TrackRequest(string name, string message, string responseCode, string userId, DateTime timeStamp, TimeSpan? duration = null, bool? success = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
		{
			Track(this.TelemetrySinksList.Select((sink) => new Action(() => sink.TrackRequest(name, message, responseCode, userId, timeStamp, duration, success, properties, metrics))));
		}

		public void TrackTrace(string message, Telemetry.Level level, string userId, DateTime timeStamp, TimeSpan? duration = null, IDictionary<string, string> properties = null)
		{
			Track(this.TelemetrySinksList.Select((sink) => new Action(() => sink.TrackTrace(message, level, userId, timeStamp, duration, properties))));
		}

		public void TrackView(string name, string message, Uri url, string userId, DateTime timeStamp, TimeSpan? duration = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
		{
			Track(this.TelemetrySinksList.Select((sink) => new Action(() => sink.TrackView(name, message, url, userId, timeStamp, duration, properties, metrics))));
		}

		#endregion
	}
}
