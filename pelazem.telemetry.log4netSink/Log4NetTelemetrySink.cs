using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using pelazem.telemetry;
using pelazem.util;

namespace pelazem.telemetry.log4netSink
{
	public class Log4NetTelemetrySink : ITelemetrySink
	{
		#region Variables

		private static ILog _logger = null;

		#endregion

		#region Constructors

		public Log4NetTelemetrySink() : this(Level.Debug, RollingFileAppender.RollingMode.Size, null, null, null, 0, null) {}

		public Log4NetTelemetrySink(Level logLevel, RollingFileAppender.RollingMode rollingMode, string repositoryName, string conversionPattern, string filePattern, long maxFileSizeInBytes, int? maxSizeRollBackups)
		{
			if (string.IsNullOrWhiteSpace(repositoryName))
				repositoryName = Properties.Resources.RepositoryName;

			if (string.IsNullOrWhiteSpace(conversionPattern))
				conversionPattern = Properties.Resources.ConversionPattern;

			if (string.IsNullOrWhiteSpace(filePattern))
				filePattern = Properties.Resources.FilePattern;

			if (maxFileSizeInBytes <= 0)
				maxFileSizeInBytes = Converter.GetInt64(Properties.Resources.MaxFileSizeInBytes);

			if (maxSizeRollBackups == null || maxSizeRollBackups <= 0)
				maxSizeRollBackups = Converter.GetInt32(Properties.Resources.MaxSizeRollBackups);

			var repo = LogManager.GetAllRepositories().Where(r => r.Name == repositoryName).FirstOrDefault() ?? LogManager.CreateRepository(repositoryName);

			Hierarchy hierarchy = repo as Hierarchy;

			hierarchy.Root.RemoveAllAppenders();

			PatternLayout patternLayout = new PatternLayout();
			patternLayout.ConversionPattern = conversionPattern;
			patternLayout.ActivateOptions();

			RollingFileAppender roller = new RollingFileAppender();
			roller.AppendToFile = false;
			roller.File = filePattern;
			roller.Layout = patternLayout;
			roller.MaxFileSize = maxFileSizeInBytes;
			roller.MaxSizeRollBackups = maxSizeRollBackups.Value;
			roller.RollingStyle = rollingMode;
			roller.StaticLogFileName = false;
			roller.ActivateOptions();
			hierarchy.Root.AddAppender(roller);

			//MemoryAppender memory = new MemoryAppender();
			//memory.ActivateOptions();
			//hierarchy.Root.AddAppender(memory);

			//hierarchy.Root.IsEnabledFor(logLevel);
			hierarchy.Root.Level = Level.All;
			hierarchy.Configured = true;

			_logger = LogManager.GetLogger(repositoryName, "Log");
		}

		public Log4NetTelemetrySink(XmlElement log4netConfig, string repositoryName = null)
		{
			if (log4netConfig == null)
				throw new ArgumentNullException(nameof(log4netConfig));

			if (string.IsNullOrWhiteSpace(repositoryName))
				repositoryName = Properties.Resources.RepositoryName;

			XmlConfigurator.Configure(LogManager.GetRepository(repositoryName), log4netConfig);
		}

		#endregion

		#region ITelemetrySink

		public void Send(ITelemetryEvent telemetryEvent)
		{
			if (telemetryEvent == null)
				throw new ArgumentNullException(nameof(telemetryEvent));

			string eventText = telemetryEvent.ToString();

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

			switch (level)
			{
				case TelemetryEvent.Levels.Critical:
					_logger.Fatal(eventText, telemetryEvent.Exception);
					break;
				case TelemetryEvent.Levels.Debug:
					_logger.Debug(eventText);
					break;
				case TelemetryEvent.Levels.Error:
					_logger.Error(eventText, telemetryEvent.Exception);
					break;
				case TelemetryEvent.Levels.Warning:
					_logger.Warn(eventText);
					break;
				case TelemetryEvent.Levels.Information:
				case TelemetryEvent.Levels.NotSpecified:
				default:
					_logger.Info(eventText);
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
