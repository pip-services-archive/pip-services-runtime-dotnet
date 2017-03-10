using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using PipServices.Runtime.Config;

namespace PipServices.Runtime.Logs
{
    public class AppInsightsLogger : AbstractLogger
    {
        private readonly TelemetryClient _client;

        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Logs, "pip-services-runtime-log", "app-insights", "*"
            );

        public AppInsightsLogger()
            : base(ClassDescriptor)
        {
            _client = new TelemetryClient();
        }

        public override void Configure(ComponentConfig config)
        {
            base.Configure(config);

            if (_config.Options.ContainsKey("instrumentation_key"))
                TelemetryConfiguration.Active.InstrumentationKey = _config.Options.GetString("instrumentation_key");
        }

        private SeverityLevel LevelToSeverity(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Fatal:
                    return SeverityLevel.Critical;
                case LogLevel.Error:
                    return SeverityLevel.Error;
                case LogLevel.Warn:
                    return SeverityLevel.Warning;
                case LogLevel.Info:
                    return SeverityLevel.Information;
                case LogLevel.Debug:
                    return SeverityLevel.Verbose;
                case LogLevel.Trace:
                    return SeverityLevel.Verbose;
            }

            return SeverityLevel.Verbose;
        }

        private void WriteToDebug(LogLevel level, string correlationId, string message)
        {
            switch (level)
            {
                case LogLevel.Fatal:
                case LogLevel.Error:
                    System.Diagnostics.Trace.TraceError(message);
                    break;
                case LogLevel.Warn:
                    System.Diagnostics.Trace.TraceWarning(message);
                    break;
                case LogLevel.Info:
                    System.Diagnostics.Trace.TraceInformation(message);
                    break;
                case LogLevel.Debug:
                case LogLevel.Trace:
                    System.Diagnostics.Debug.WriteLine(message);
                    break;
            }
        }

        public override void Log(LogLevel level, string component, string correlationId, object[] message)
        {
            var output = LogFormatter.Format(level, message);
            if (correlationId != null)
                output += ", correlated to " + correlationId;

            WriteToDebug(level, correlationId, output);

            if (level == LogLevel.Error || level == LogLevel.Fatal)
            {
                var props = new Dictionary<string, string> {{"CorrelationId", correlationId}};

                //ToDo exception logging
                //_client.TrackException(error, props);
            }

            _client.TrackTrace(output, LevelToSeverity(level));
        }

        public void Dump()
        {
            _client.Flush();
        }
    }
}
