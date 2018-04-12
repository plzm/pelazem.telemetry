using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using pelazem.telemetry;

namespace pelazem.telemetry.eventGridSink
{
	public class EventGridTelemetrySink : ITelemetrySink
	{
		#region Variables

		private TopicCredentials _topicCredentials = null;

		private EventGridClient _eventGridClient = null;

		#endregion

		#region Properties

		public string EventGridTopicHostName { get; set; }

		public string EventGridTopicKey { get; set; }

		public TopicCredentials TopicCredentials
		{
			get
			{
				if (_topicCredentials == null)
					_topicCredentials = new TopicCredentials(this.EventGridTopicKey);

				return _topicCredentials;
			}
		}

		public EventGridClient EventGridClient
		{
			get
			{
				if (_eventGridClient == null)
					_eventGridClient = new EventGridClient(this.TopicCredentials);

				return _eventGridClient;
			}
		}

		#endregion

		#region Constructors

		private EventGridTelemetrySink() { }

		public EventGridTelemetrySink(string eventGridTopicHostName, string eventGridTopicKey)
		{
			if (string.IsNullOrWhiteSpace(eventGridTopicHostName))
				throw new ArgumentException(Properties.Resources.EventGridTopicHostName, nameof(eventGridTopicHostName));

			if (string.IsNullOrWhiteSpace(eventGridTopicKey))
				throw new ArgumentException(Properties.Resources.EventGridTopicKey, nameof(eventGridTopicKey));

			this.EventGridTopicHostName = eventGridTopicHostName;
			this.EventGridTopicKey = eventGridTopicKey;
		}

		#endregion

		#region ITelemetrySink

		public void Send(ITelemetryEvent telemetryEvent)
		{
			if (telemetryEvent == null)
				throw new ArgumentNullException(nameof(telemetryEvent));

			Send(new ITelemetryEvent[] { telemetryEvent });
		}

		public async void Send(IEnumerable<ITelemetryEvent> telemetryEvents)
		{
			if (telemetryEvents == null)
				throw new ArgumentNullException(nameof(telemetryEvents));

			if (telemetryEvents.Count() == 0)
				throw new ArgumentException(Properties.Resources.EventListEmpty, nameof(telemetryEvents));

			List<EventGridEvent> eventGridEvents = telemetryEvents.Select(te => new EventGridEvent(te.Id ?? "-1", te.Name, te, te.EventType, (te.TimeStamp != null && te.TimeStamp.HasValue ? te.TimeStamp.Value : DateTime.UtcNow), te.Version)).ToList();

			AzureOperationResponse response = await this.EventGridClient.PublishEventsWithHttpMessagesAsync(this.EventGridTopicHostName, eventGridEvents);
		}

		#endregion
	}
}
