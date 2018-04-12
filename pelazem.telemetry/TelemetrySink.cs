using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pelazem.telemetry
{
	public class TelemetrySink : ITelemetrySink
	{
		#region Properties

		private List<ITelemetrySink> TelemetrySinksList { get; set; } = new List<ITelemetrySink>();

		public IEnumerable<ITelemetrySink> TelemetrySinks
		{
			get { return this.TelemetrySinksList.AsEnumerable(); }
		}

		#endregion

		#region Constructors

		private TelemetrySink() { }

		public TelemetrySink(ITelemetrySink telemetrySink)
		{
			if (telemetrySink == null)
				throw new ArgumentNullException();

			this.TelemetrySinksList.Add(telemetrySink);
		}

		public TelemetrySink(IEnumerable<ITelemetrySink> telemetrySinks)
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

		#region ITelemetrySink

		public void Send(ITelemetryEvent telemetryEvent)
		{
			if (telemetryEvent == null)
				throw new ArgumentNullException(nameof(telemetryEvent));

			Send(new ITelemetryEvent[] { telemetryEvent });
		}

		public void Send(IEnumerable<ITelemetryEvent> telemetryEvents)
		{
			if (telemetryEvents == null)
				throw new ArgumentNullException(nameof(telemetryEvents));
			if (telemetryEvents.Count() == 0)
				throw new ArgumentException(Properties.Resources.EventListEmpty, nameof(telemetryEvents));

			var actions = this.TelemetrySinksList.Select((sink) => new Action(() => sink.Send(telemetryEvents)));

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
	}
}
