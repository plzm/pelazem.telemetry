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
	public class TelemetrySink : ITelemetrySink
	{
		#region Variables

		private static ILog _logger = null;

		#endregion

		#region Properties

		#endregion

		#region Constructors

		public TelemetrySink() : this(Level.Debug, RollingFileAppender.RollingMode.Size, null, null, null, 0, null) {}

		public TelemetrySink(Level logLevel, RollingFileAppender.RollingMode rollingMode, string repositoryName, string conversionPattern, string filePattern, long maxFileSizeInBytes, int? maxSizeRollBackups)
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

		public TelemetrySink(XmlElement log4netConfig, string repositoryName = null)
		{
			if (log4netConfig == null)
				throw new ArgumentNullException(nameof(log4netConfig));

			if (string.IsNullOrWhiteSpace(repositoryName))
				repositoryName = Properties.Resources.RepositoryName;

			XmlConfigurator.Configure(LogManager.GetRepository(repositoryName), log4netConfig);
		}

		#endregion

		#region Methods

		#endregion

		#region ITelemetrySink

		public string Name { get { return Properties.Resources.Name; } }

		public void TrackDependency(string type, string name, string message, string target, string result, string userId, DateTime timeStamp, TimeSpan? duration = null, bool? success = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
		{
			LogEvent e = new LogEvent();

			e.EventType = nameof(TrackDependency);

			if (!string.IsNullOrWhiteSpace(type))
				e.Properties.Add("DependencyType", type);

			e.Name = name;
			e.Message = message;

			if (!string.IsNullOrWhiteSpace(target))
				e.Properties.Add("Target", target);

			e.UserId = userId;
			e.TimeStamp = timeStamp;
			e.Duration = duration;
			e.Success = success;

			if (properties != null)
				e.Properties.AddItems(properties);

			if (metrics != null)
				e.Metrics.AddItems(metrics);

			_logger.Info(e.ToString());
		}

		public void TrackEvent(string name, string message, string result, Telemetry.Level level, string userId, DateTime timeStamp, TimeSpan? duration = null, bool? success = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
		{
			LogEvent e = new LogEvent();

			e.EventType = nameof(TrackEvent);
			e.Name = name;
			e.Message = message;
			e.Result = result;
			e.Properties.Add("Level", Enum.GetName(typeof(Telemetry.Level), level));
			e.UserId = userId;
			e.TimeStamp = timeStamp;
			e.Duration = duration;
			e.Success = success;

			if (properties != null)
				e.Properties.AddItems(properties);

			if (metrics != null)
				e.Metrics.AddItems(metrics);

			if (level == Telemetry.Level.Debug)
				_logger.Debug(e.ToString());
			else if (level == Telemetry.Level.Error)
				_logger.Error(e.ToString());
			else if (level == Telemetry.Level.Critical)
				_logger.Warn(e.ToString());
			else
				_logger.Info(e.ToString());
		}

		public void TrackException(Exception exception, string message, string problemId, string userId, DateTime timeStamp, TimeSpan? duration = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
		{
			LogEvent e = new LogEvent();

			e.EventType = nameof(TrackException);
			e.Message = message;

			if (!string.IsNullOrWhiteSpace(problemId))
				e.Properties.Add("ProblemId", problemId);

			e.UserId = userId;
			e.TimeStamp = timeStamp;
			e.Duration = duration;

			if (properties != null)
				e.Properties.AddItems(properties);

			if (metrics != null)
				e.Metrics.AddItems(metrics);

			_logger.Error(e.ToString(), exception);
		}

		public void TrackMetric(string metricName, double metricValue, string userId, DateTime timeStamp)
		{
			LogEvent e = new LogEvent();

			e.EventType = nameof(TrackMetric);
			e.UserId = userId;
			e.TimeStamp = timeStamp;

			e.Metrics.Add(metricName, metricValue);

			_logger.Info(e.ToString());
		}

		public void TrackRequest(string name, string message, string responseCode, string userId, DateTime timeStamp, TimeSpan? duration = null, bool? success = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
		{
			LogEvent e = new LogEvent();

			e.EventType = nameof(TrackRequest);
			e.Name = name;
			e.Message = message;

			if (!string.IsNullOrWhiteSpace(responseCode))
				e.Properties.Add("ResponseCode", responseCode);

			e.UserId = userId;
			e.TimeStamp = timeStamp;
			e.Duration = duration;
			e.Success = success;

			if (properties != null)
				e.Properties.AddItems(properties);

			if (metrics != null)
				e.Metrics.AddItems(metrics);

			_logger.Info(e.ToString());
		}

		public void TrackTrace(string message, Telemetry.Level level, string userId, DateTime timeStamp, TimeSpan? duration = null, IDictionary<string, string> properties = null)
		{
			LogEvent e = new LogEvent();

			e.EventType = nameof(TrackTrace);
			e.Message = message;
			e.Properties.Add("Level", Enum.GetName(typeof(Telemetry.Level), level));
			e.UserId = userId;
			e.TimeStamp = timeStamp;
			e.Duration = duration;

			if (properties != null)
				e.Properties.AddItems(properties);

			if (level == Telemetry.Level.Debug)
				_logger.Debug(e.ToString());
			else if (level == Telemetry.Level.Error)
				_logger.Error(e.ToString());
			else if (level == Telemetry.Level.Critical)
				_logger.Warn(e.ToString());
			else
				_logger.Info(e.ToString());
		}

		public void TrackView(string name, string message, Uri url, string userId, DateTime timeStamp, TimeSpan? duration = null, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
		{
			LogEvent e = new LogEvent();

			e.EventType = nameof(TrackView);
			e.Name = name;
			e.Message = message;
			e.Uri = url;
			e.UserId = userId;
			e.TimeStamp = timeStamp;
			e.Duration = duration;

			if (properties != null)
				e.Properties.AddItems(properties);

			if (metrics != null)
				e.Metrics.AddItems(metrics);

			_logger.Info(e.ToString());
		}

		#endregion
	}
}
