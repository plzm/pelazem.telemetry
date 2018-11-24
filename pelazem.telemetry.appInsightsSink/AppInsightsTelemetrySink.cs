using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using pelazem.telemetry;
using pelazem.util;

namespace pelazem.telemetry.appInsightsSink
{
	public class AppInsightsTelemetrySink : ITelemetrySink
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

		private AppInsightsTelemetrySink() { }

		public AppInsightsTelemetrySink(string appInsightsInstrumentationKey)
		{
			if (string.IsNullOrWhiteSpace(appInsightsInstrumentationKey))
				throw new ArgumentException(Properties.Resources.AppInsightsKeyBlank);

			this.AppInsightsInstrumentationKey = appInsightsInstrumentationKey;
		}

		#endregion

		#region Methods

		private SeverityLevel GetSeverityLevel(ITelemetryEvent telemetryEvent)
		{
			if (telemetryEvent == null)
				return SeverityLevel.Information;

			TelemetryEvent.Levels level;

			if (telemetryEvent is TelemetryEvent)
				level = (telemetryEvent as TelemetryEvent).LevelValue;
			else
			{
				if (!string.IsNullOrWhiteSpace(telemetryEvent.Level))
					Enum.TryParse(telemetryEvent.Level, out level);
				else
					level = TelemetryEvent.Levels.NotSpecified;
			}

			SeverityLevel result;

			switch (level)
			{
				case TelemetryEvent.Levels.Critical:
					result = SeverityLevel.Critical;
					break;
				case TelemetryEvent.Levels.Debug:
					result = SeverityLevel.Verbose;
					break;
				case TelemetryEvent.Levels.Error:
					result = SeverityLevel.Error;
					break;
				case TelemetryEvent.Levels.Information:
					result = SeverityLevel.Information;
					break;
				case TelemetryEvent.Levels.Warning:
					result = SeverityLevel.Warning;
					break;
				default:
					result = SeverityLevel.Information;
					break;
			}

			return result;
		}

		private void TrackDependency(ITelemetryEvent telemetryEvent)
		{
			DependencyTelemetry info = new DependencyTelemetry();

			info.Data = telemetryEvent.Data;

			if (telemetryEvent.Duration != null && telemetryEvent.Duration.HasValue)
				info.Duration = telemetryEvent.Duration.Value;

			info.Type = telemetryEvent.EventType;

			info.Id = telemetryEvent.Id;

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Level))
				info.Properties.Add(nameof(telemetryEvent.Level), telemetryEvent.Level);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Message))
				info.Properties.Add(nameof(telemetryEvent.Message), telemetryEvent.Message);

			info.Metrics.AddItems(telemetryEvent.Metrics);

			info.Name = telemetryEvent.Name;

			if (!string.IsNullOrWhiteSpace(telemetryEvent.ProblemId))
				info.Properties.Add(nameof(telemetryEvent.ProblemId), telemetryEvent.ProblemId);

			info.Properties.AddItems(telemetryEvent.Properties);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Protocol))
				info.Properties.Add(nameof(telemetryEvent.Protocol), telemetryEvent.Protocol);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.ResponseCode))
				info.Properties.Add(nameof(telemetryEvent.ResponseCode), telemetryEvent.ResponseCode);

			info.ResultCode = telemetryEvent.Result;

			info.Success = telemetryEvent.Success;

			info.Target = telemetryEvent.Target;

			if (telemetryEvent.TimeStamp != null)
				info.Timestamp = telemetryEvent.TimeStamp.Value;

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Uri))
				info.Properties.Add(nameof(telemetryEvent.Uri), telemetryEvent.Uri);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.UserId))
				info.Context.User.Id = telemetryEvent.UserId;

			if (telemetryEvent.Value != null)
				info.Properties.Add(nameof(telemetryEvent.Value), telemetryEvent.Value.Value.ToString());

			this.Client.TrackDependency(info);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.UserId))
				info.Context.User.Id = string.Empty;
		}

		private void TrackEvent(ITelemetryEvent telemetryEvent)
		{
			EventTelemetry info = new EventTelemetry();

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Data))
				info.Properties.Add(nameof(telemetryEvent.Data), telemetryEvent.Data);

			if (telemetryEvent.Duration != null && telemetryEvent.Duration.HasValue)
				info.Properties.Add(nameof(telemetryEvent.Duration), telemetryEvent.Duration.Value.ToString());

			if (!string.IsNullOrWhiteSpace(telemetryEvent.EventType))
				info.Properties.Add(nameof(telemetryEvent.EventType), telemetryEvent.EventType);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Id))
				info.Properties.Add(nameof(telemetryEvent.Id), telemetryEvent.Id);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Level))
				info.Properties.Add(nameof(telemetryEvent.Level), telemetryEvent.Level);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Message))
				info.Properties.Add(nameof(telemetryEvent.Message), telemetryEvent.Message);

			info.Metrics.AddItems(telemetryEvent.Metrics);

			info.Name = telemetryEvent.Name;

			if (!string.IsNullOrWhiteSpace(telemetryEvent.ProblemId))
				info.Properties.Add(nameof(telemetryEvent.ProblemId), telemetryEvent.ProblemId);

			info.Properties.AddItems(telemetryEvent.Properties);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Protocol))
				info.Properties.Add(nameof(telemetryEvent.Protocol), telemetryEvent.Protocol);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.ResponseCode))
				info.Properties.Add(nameof(telemetryEvent.ResponseCode), telemetryEvent.ResponseCode);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Result))
				info.Properties.Add(nameof(telemetryEvent.Result), telemetryEvent.Result);

			if (telemetryEvent.Success != null)
				info.Properties.Add(nameof(telemetryEvent.Success), telemetryEvent.Success.Value.ToString());

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Target))
				info.Properties.Add(nameof(telemetryEvent.Target), telemetryEvent.Target);

			if (telemetryEvent.TimeStamp != null)
				info.Timestamp = telemetryEvent.TimeStamp.Value;

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Uri))
				info.Properties.Add(nameof(telemetryEvent.Uri), telemetryEvent.Uri);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.UserId))
				info.Context.User.Id = telemetryEvent.UserId;

			if (telemetryEvent.Value != null)
				info.Properties.Add(nameof(telemetryEvent.Value), telemetryEvent.Value.Value.ToString());

			this.Client.TrackEvent(info);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.UserId))
				info.Context.User.Id = string.Empty;
		}

		private void TrackException(ITelemetryEvent telemetryEvent)
		{
			ExceptionTelemetry info = new ExceptionTelemetry() { SeverityLevel = SeverityLevel.Error };

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Data))
				info.Properties.Add(nameof(telemetryEvent.Data), telemetryEvent.Data);

			if (telemetryEvent.Duration != null && telemetryEvent.Duration.HasValue)
				info.Properties.Add(nameof(telemetryEvent.Duration), telemetryEvent.Duration.Value.ToString());

			if (!string.IsNullOrWhiteSpace(telemetryEvent.EventType))
				info.Properties.Add(nameof(telemetryEvent.EventType), telemetryEvent.EventType);

			if (telemetryEvent.Exception == null)
				info.Exception = telemetryEvent.Exception;

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Id))
				info.Properties.Add(nameof(telemetryEvent.Id), telemetryEvent.Id);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Level))
				info.Properties.Add(nameof(telemetryEvent.Level), telemetryEvent.Level);

			info.Message = telemetryEvent.Message;

			info.Metrics.AddItems(telemetryEvent.Metrics);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Name))
				info.Properties.Add(nameof(telemetryEvent.Name), telemetryEvent.Name);

			info.ProblemId = telemetryEvent.ProblemId;

			info.Properties.AddItems(telemetryEvent.Properties);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Protocol))
				info.Properties.Add(nameof(telemetryEvent.Protocol), telemetryEvent.Protocol);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.ResponseCode))
				info.Properties.Add(nameof(telemetryEvent.ResponseCode), telemetryEvent.ResponseCode);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Result))
				info.Properties.Add(nameof(telemetryEvent.Result), telemetryEvent.Result);

			if (telemetryEvent.Success != null)
				info.Properties.Add(nameof(telemetryEvent.Success), telemetryEvent.Success.Value.ToString());

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Target))
				info.Properties.Add(nameof(telemetryEvent.Target), telemetryEvent.Target);

			if (telemetryEvent.TimeStamp != null)
				info.Timestamp = telemetryEvent.TimeStamp.Value;

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Uri))
				info.Properties.Add(nameof(telemetryEvent.Uri), telemetryEvent.Uri);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.UserId))
				info.Context.User.Id = telemetryEvent.UserId;

			if (telemetryEvent.Value != null)
				info.Properties.Add(nameof(telemetryEvent.Value), telemetryEvent.Value.Value.ToString());

			this.Client.TrackException(info);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.UserId))
				info.Context.User.Id = string.Empty;
		}

		private void TrackMetric(ITelemetryEvent telemetryEvent)
		{
			MetricTelemetry info = new MetricTelemetry();

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Data))
				info.Properties.Add(nameof(telemetryEvent.Data), telemetryEvent.Data);

			if (telemetryEvent.Duration != null && telemetryEvent.Duration.HasValue)
				info.Properties.Add(nameof(telemetryEvent.Duration), telemetryEvent.Duration.Value.ToString());

			if (!string.IsNullOrWhiteSpace(telemetryEvent.EventType))
				info.Properties.Add(nameof(telemetryEvent.EventType), telemetryEvent.EventType);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Id))
				info.Properties.Add(nameof(telemetryEvent.Id), telemetryEvent.Id);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Level))
				info.Properties.Add(nameof(telemetryEvent.Level), telemetryEvent.Level);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Message))
				info.Properties.Add(nameof(telemetryEvent.Message), telemetryEvent.Message);

			info.Name = telemetryEvent.Name;

			if (!string.IsNullOrWhiteSpace(telemetryEvent.ProblemId))
				info.Properties.Add(nameof(telemetryEvent.ProblemId), telemetryEvent.ProblemId);

			info.Properties.AddItems(telemetryEvent.Properties);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Protocol))
				info.Properties.Add(nameof(telemetryEvent.Protocol), telemetryEvent.Protocol);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.ResponseCode))
				info.Properties.Add(nameof(telemetryEvent.ResponseCode), telemetryEvent.ResponseCode);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Result))
				info.Properties.Add(nameof(telemetryEvent.Result), telemetryEvent.Result);

			if (telemetryEvent.Success != null)
				info.Properties.Add(nameof(telemetryEvent.Success), telemetryEvent.Success.Value.ToString());

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Target))
				info.Properties.Add(nameof(telemetryEvent.Target), telemetryEvent.Target);

			if (telemetryEvent.TimeStamp != null)
				info.Timestamp = telemetryEvent.TimeStamp.Value;

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Uri))
				info.Properties.Add(nameof(telemetryEvent.Uri), telemetryEvent.Uri);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.UserId))
				info.Context.User.Id = telemetryEvent.UserId;

			if (telemetryEvent.Value != null)
				info.Properties.Add(nameof(telemetryEvent.Value), telemetryEvent.Value.Value.ToString());

			this.Client.TrackMetric(info);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.UserId))
				info.Context.User.Id = string.Empty;
		}

		private void TrackRequest(ITelemetryEvent telemetryEvent)
		{
			RequestTelemetry info = new RequestTelemetry();

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Data))
				info.Properties.Add(nameof(telemetryEvent.Data), telemetryEvent.Data);

			if (telemetryEvent.Duration != null && telemetryEvent.Duration.HasValue)
				info.Properties.Add(nameof(telemetryEvent.Duration), telemetryEvent.Duration.Value.ToString());

			if (!string.IsNullOrWhiteSpace(telemetryEvent.EventType))
				info.Properties.Add(nameof(telemetryEvent.EventType), telemetryEvent.EventType);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Id))
				info.Properties.Add(nameof(telemetryEvent.Id), telemetryEvent.Id);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Level))
				info.Properties.Add(nameof(telemetryEvent.Level), telemetryEvent.Level);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Message))
				info.Properties.Add(nameof(telemetryEvent.Message), telemetryEvent.Message);

			info.Metrics.AddItems(telemetryEvent.Metrics);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Name))
				info.Properties.Add(nameof(telemetryEvent.Name), telemetryEvent.Name);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.ProblemId))
				info.Properties.Add(nameof(telemetryEvent.ProblemId), telemetryEvent.ProblemId);

			info.Properties.AddItems(telemetryEvent.Properties);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Protocol))
				info.Properties.Add(nameof(telemetryEvent.Protocol), telemetryEvent.Protocol);

			info.ResponseCode = telemetryEvent.ResponseCode;

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Result))
				info.Properties.Add(nameof(telemetryEvent.Result), telemetryEvent.Result);

			if (telemetryEvent.Success != null)
				info.Properties.Add(nameof(telemetryEvent.Success), telemetryEvent.Success.Value.ToString());

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Target))
				info.Properties.Add(nameof(telemetryEvent.Target), telemetryEvent.Target);

			if (telemetryEvent.TimeStamp != null)
				info.Timestamp = telemetryEvent.TimeStamp.Value;

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Uri))
				info.Properties.Add(nameof(telemetryEvent.Uri), telemetryEvent.Uri);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.UserId))
				info.Context.User.Id = telemetryEvent.UserId;

			if (telemetryEvent.Value != null)
				info.Properties.Add(nameof(telemetryEvent.Value), telemetryEvent.Value.Value.ToString());

			this.Client.TrackRequest(info);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.UserId))
				info.Context.User.Id = string.Empty;
		}

		private void TrackTrace(ITelemetryEvent telemetryEvent)
		{
			TraceTelemetry info = new TraceTelemetry();

			info.SeverityLevel = GetSeverityLevel(telemetryEvent);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Data))
				info.Properties.Add(nameof(telemetryEvent.Data), telemetryEvent.Data);

			if (telemetryEvent.Duration != null && telemetryEvent.Duration.HasValue)
				info.Properties.Add(nameof(telemetryEvent.Duration), telemetryEvent.Duration.Value.ToString());

			if (!string.IsNullOrWhiteSpace(telemetryEvent.EventType))
				info.Properties.Add(nameof(telemetryEvent.EventType), telemetryEvent.EventType);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Id))
				info.Properties.Add(nameof(telemetryEvent.Id), telemetryEvent.Id);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Level))
				info.Properties.Add(nameof(telemetryEvent.Level), telemetryEvent.Level);

			info.Message = telemetryEvent.Message;

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Name))
				info.Properties.Add(nameof(telemetryEvent.Name), telemetryEvent.Name);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.ProblemId))
				info.Properties.Add(nameof(telemetryEvent.ProblemId), telemetryEvent.ProblemId);

			info.Properties.AddItems(telemetryEvent.Properties);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Protocol))
				info.Properties.Add(nameof(telemetryEvent.Protocol), telemetryEvent.Protocol);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.ResponseCode))
				info.Properties.Add(nameof(telemetryEvent.ResponseCode), telemetryEvent.ResponseCode);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Result))
				info.Properties.Add(nameof(telemetryEvent.Result), telemetryEvent.Result);

			if (telemetryEvent.Success != null)
				info.Properties.Add(nameof(telemetryEvent.Success), telemetryEvent.Success.Value.ToString());

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Target))
				info.Properties.Add(nameof(telemetryEvent.Target), telemetryEvent.Target);

			if (telemetryEvent.TimeStamp != null)
				info.Timestamp = telemetryEvent.TimeStamp.Value;

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Uri))
				info.Properties.Add(nameof(telemetryEvent.Uri), telemetryEvent.Uri);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.UserId))
				info.Context.User.Id = telemetryEvent.UserId;

			if (telemetryEvent.Value != null)
				info.Properties.Add(nameof(telemetryEvent.Value), telemetryEvent.Value.Value.ToString());

			this.Client.TrackTrace(info);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.UserId))
				info.Context.User.Id = string.Empty;
		}

		private void TrackView(ITelemetryEvent telemetryEvent)
		{
			PageViewTelemetry info = new PageViewTelemetry();

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Data))
				info.Properties.Add(nameof(telemetryEvent.Data), telemetryEvent.Data);

			if (telemetryEvent.Duration != null && telemetryEvent.Duration.HasValue)
				info.Properties.Add(nameof(telemetryEvent.Duration), telemetryEvent.Duration.Value.ToString());

			if (!string.IsNullOrWhiteSpace(telemetryEvent.EventType))
				info.Properties.Add(nameof(telemetryEvent.EventType), telemetryEvent.EventType);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Id))
				info.Properties.Add(nameof(telemetryEvent.Id), telemetryEvent.Id);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Level))
				info.Properties.Add(nameof(telemetryEvent.Level), telemetryEvent.Level);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Message))
				info.Properties.Add(nameof(telemetryEvent.Message), telemetryEvent.Message);

			info.Metrics.AddItems(telemetryEvent.Metrics);

			info.Name = telemetryEvent.Name;

			if (!string.IsNullOrWhiteSpace(telemetryEvent.ProblemId))
				info.Properties.Add(nameof(telemetryEvent.ProblemId), telemetryEvent.ProblemId);

			info.Properties.AddItems(telemetryEvent.Properties);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Protocol))
				info.Properties.Add(nameof(telemetryEvent.Protocol), telemetryEvent.Protocol);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.ResponseCode))
				info.Properties.Add(nameof(telemetryEvent.ResponseCode), telemetryEvent.ResponseCode);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Result))
				info.Properties.Add(nameof(telemetryEvent.Result), telemetryEvent.Result);

			if (telemetryEvent.Success != null)
				info.Properties.Add(nameof(telemetryEvent.Success), telemetryEvent.Success.Value.ToString());

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Target))
				info.Properties.Add(nameof(telemetryEvent.Target), telemetryEvent.Target);

			if (telemetryEvent.TimeStamp != null)
				info.Timestamp = telemetryEvent.TimeStamp.Value;

			if (!string.IsNullOrWhiteSpace(telemetryEvent.Uri))
				info.Url = new Uri(telemetryEvent.Uri);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.UserId))
				info.Context.User.Id = telemetryEvent.UserId;

			if (telemetryEvent.Value != null)
				info.Properties.Add(nameof(telemetryEvent.Value), telemetryEvent.Value.Value.ToString());

			this.Client.TrackPageView(info);

			if (!string.IsNullOrWhiteSpace(telemetryEvent.UserId))
				info.Context.User.Id = string.Empty;
		}

		#endregion

		#region ITelemetrySink

		public void Send(ITelemetryEvent telemetryEvent)
		{
			if (telemetryEvent == null)
				throw new ArgumentNullException(nameof(telemetryEvent));

			TelemetryEvent.EventTypes eventType;

			if (telemetryEvent is TelemetryEvent)
			{
				eventType = (telemetryEvent as TelemetryEvent).EventTypeValue;
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(telemetryEvent.EventType))
					Enum.TryParse(telemetryEvent.EventType, out eventType);
				else
					eventType = TelemetryEvent.EventTypes.NotSpecified;
			}

			switch (eventType)
			{
				case TelemetryEvent.EventTypes.Dependency:
					TrackDependency(telemetryEvent);
					break;
				case TelemetryEvent.EventTypes.Event:
				case TelemetryEvent.EventTypes.NotSpecified:
					TrackEvent(telemetryEvent);
					break;
				case TelemetryEvent.EventTypes.Exception:
					TrackException(telemetryEvent);
					break;
				case TelemetryEvent.EventTypes.Metric:
					TrackMetric(telemetryEvent);
					break;
				case TelemetryEvent.EventTypes.Request:
					TrackRequest(telemetryEvent);
					break;
				case TelemetryEvent.EventTypes.Trace:
					TrackTrace(telemetryEvent);
					break;
				case TelemetryEvent.EventTypes.View:
					TrackView(telemetryEvent);
					break;
				default:
					TrackEvent(telemetryEvent);
					break;
			}
		}

		public void Send(IEnumerable<ITelemetryEvent> telemetryEvents)
		{
			if (telemetryEvents == null)
				throw new ArgumentNullException(nameof(telemetryEvents));

			if (telemetryEvents.Count() == 0)
				throw new ArgumentException(Properties.Resources.EventListEmpty, nameof(telemetryEvents));

			foreach (TelemetryEvent te in telemetryEvents)
				Send(te);
		}

		#endregion
	}
}
