using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using pelazem.telemetry;

namespace pelazem.telemetry.appInsightsSink
{
	public class TelemetrySink : ITelemetrySink
	{
		#region Variables

		private TelemetryClient _client;

		#endregion

		#region Properties

		private string AppInsightsInstrumentationKey { get; set; }

		public TelemetryClient Client
		{
			get
			{
				if (_client == null)
				{
					TelemetryConfiguration.Active.InstrumentationKey = this.AppInsightsInstrumentationKey;

					_client = new TelemetryClient() { InstrumentationKey = this.AppInsightsInstrumentationKey };
				}

				return _client;
			}
		}

		#endregion

		#region Constructors

		private TelemetrySink() { }

		public TelemetrySink(string appInsightsInstrumentationKey)
		{
			if (string.IsNullOrWhiteSpace(appInsightsInstrumentationKey))
				throw new ArgumentException(Properties.Resources.AppInsightsKeyBlank);

			this.AppInsightsInstrumentationKey = appInsightsInstrumentationKey;
		}

		#endregion

		#region Methods

		private SeverityLevel GetSeverityLevel(Telemetry.Level level)
		{
			SeverityLevel result;

			switch (level)
			{
				case Telemetry.Level.Critical:
					result = SeverityLevel.Critical;
					break;
				case Telemetry.Level.Error:
					result = SeverityLevel.Error;
					break;
				case Telemetry.Level.Information:
					result = SeverityLevel.Information;
					break;
				case Telemetry.Level.Debug:
					result = SeverityLevel.Verbose;
					break;
				case Telemetry.Level.Warning:
					result = SeverityLevel.Warning;
					break;
				default:
					result = SeverityLevel.Information;
					break;
			}

			return result;
		}

		#endregion

		#region ITelemetrySink

		public string Name { get { return Properties.Resources.Name; } }

		public void TrackDependency(string type, string name, string message, string target, string result, string userId, DateTime timeStamp, TimeSpan? duration = null, bool? success = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
		{
			DependencyTelemetry info = new DependencyTelemetry();

			Prep(info, name, timeStamp, duration, success, properties, metrics);

			info.Type = type;
			info.Target = target;
			info.Data = message;
			info.ResultCode = result;

			if (!string.IsNullOrWhiteSpace(userId))
				info.Context.User.Id = userId;

			this.Client.TrackDependency(info);

			if (!string.IsNullOrWhiteSpace(userId))
				info.Context.User.Id = string.Empty;
		}

		public void TrackEvent(string name, string message, string result, Telemetry.Level level, string userId, DateTime timeStamp, TimeSpan? duration = null, bool? success = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
		{
			var info = new EventTelemetry();

			info.Name = name;

			if (!string.IsNullOrWhiteSpace(message))
				info.Properties[nameof(message)] = message;

			if (!string.IsNullOrWhiteSpace(result))
				info.Properties[nameof(result)] = result;

			info.Properties[nameof(level)] = Enum.GetName(typeof(Telemetry.Level), level);

			info.Timestamp = timeStamp;

			if (duration != null && duration > TimeSpan.MinValue)
				info.Properties[nameof(duration)] = duration.ToString();

			if (success != null)
				info.Properties[nameof(success)] = success.Value.ToString();

			if (properties != null)
			{
				foreach (string key in properties.Keys)
					info.Properties.Add(key, properties[key]);
			}

			if (metrics != null)
			{
				foreach (string key in metrics.Keys)
					info.Metrics.Add(key, metrics[key]);
			}

			if (!string.IsNullOrWhiteSpace(userId))
				info.Context.User.Id = userId;

			this.Client.TrackEvent(info);

			if (!string.IsNullOrWhiteSpace(userId))
				info.Context.User.Id = string.Empty;
		}

		public void TrackException(Exception exception, string message, string problemId, string userId, DateTime timeStamp, TimeSpan? duration = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
		{
			if (exception == null)
				throw new ArgumentNullException(nameof(exception));

			ExceptionTelemetry info = new ExceptionTelemetry();
			info.SeverityLevel = SeverityLevel.Error;

			info.Exception = exception;

			if (!string.IsNullOrWhiteSpace(message))
				info.Message = message;
			else
				info.Message = exception.Message;

			info.ProblemId = problemId;
			info.Timestamp = timeStamp;

			if (duration != null && duration > TimeSpan.MinValue)
				info.Properties[nameof(duration)] = duration.ToString();

			if (properties != null)
			{
				foreach (string key in properties.Keys)
					info.Properties.Add(key, properties[key]);
			}

			if (metrics != null)
			{
				foreach (string key in metrics.Keys)
					info.Metrics.Add(key, metrics[key]);
			}

			if (!string.IsNullOrWhiteSpace(userId))
				info.Context.User.Id = userId;

			this.Client.TrackException(info);

			if (!string.IsNullOrWhiteSpace(userId))
				info.Context.User.Id = string.Empty;
		}

		public void TrackMetric(string metricName, double metricValue, string userId, DateTime timeStamp)
		{
			MetricTelemetry info = new MetricTelemetry(metricName, metricValue);
			info.Timestamp = timeStamp;

			if (!string.IsNullOrWhiteSpace(userId))
				info.Context.User.Id = userId;

			this.Client.TrackMetric(info);

			if (!string.IsNullOrWhiteSpace(userId))
				info.Context.User.Id = string.Empty;
		}

		public void TrackRequest(string name, string message, string responseCode, string userId, DateTime timeStamp, TimeSpan? duration = null, bool? success = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
		{
			RequestTelemetry info = new RequestTelemetry();

			Prep(info, name, timeStamp, duration, success, properties, metrics);

			if (!string.IsNullOrWhiteSpace(message))
				info.Properties[nameof(message)] = message;

			info.ResponseCode = responseCode;

			if (!string.IsNullOrWhiteSpace(userId))
				info.Context.User.Id = userId;

			this.Client.TrackRequest(info);

			if (!string.IsNullOrWhiteSpace(userId))
				info.Context.User.Id = string.Empty;
		}

		public void TrackTrace(string message, Telemetry.Level level, string userId, DateTime timeStamp, TimeSpan? duration = null, IDictionary<string, string> properties = null)
		{
			TraceTelemetry info = new TraceTelemetry();

			info.Message = message;
			info.SeverityLevel = GetSeverityLevel(level);
			info.Timestamp = timeStamp;

			if (duration != null && duration > TimeSpan.MinValue)
				info.Properties[nameof(duration)] = duration.ToString();

			if (properties != null)
			{
				foreach (string key in properties.Keys)
					info.Properties.Add(key, properties[key]);
			}

			if (!string.IsNullOrWhiteSpace(userId))
				info.Context.User.Id = userId;

			this.Client.TrackTrace(info);

			if (!string.IsNullOrWhiteSpace(userId))
				info.Context.User.Id = string.Empty;
		}

		public void TrackView(string name, string message, Uri url, string userId, DateTime timeStamp, TimeSpan? duration = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
		{
			PageViewTelemetry info = new PageViewTelemetry();

			info.Name = name;

			if (!string.IsNullOrWhiteSpace(message))
				info.Properties[nameof(message)] = message;

			info.Url = url;
			info.Timestamp = timeStamp;

			if (duration != null && duration > TimeSpan.MinValue)
				info.Properties[nameof(duration)] = duration.ToString();

			if (properties != null)
			{
				foreach (string key in properties.Keys)
					info.Properties.Add(key, properties[key]);
			}

			if (metrics != null)
			{
				foreach (string key in metrics.Keys)
					info.Metrics.Add(key, metrics[key]);
			}

			if (!string.IsNullOrWhiteSpace(userId))
				info.Context.User.Id = userId;

			this.Client.TrackPageView(info);

			if (!string.IsNullOrWhiteSpace(userId))
				info.Context.User.Id = string.Empty;
		}

		private void Prep(OperationTelemetry info, string name, DateTime timeStamp, TimeSpan? duration = null, bool? success = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
		{
			info.Name = name;

			info.Timestamp = timeStamp;

			if (duration != null)
				info.Duration = duration.Value;

			info.Success = success;

			if (properties != null)
			{
				foreach (string key in properties.Keys)
					info.Properties.Add(key, properties[key]);
			}

			if (metrics != null)
			{
				foreach (string key in metrics.Keys)
					info.Metrics.Add(key, metrics[key]);
			}
		}

		#endregion
	}
}
