using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using PipServices.Runtime.Config;

namespace PipServices.Runtime.Counters
{
    public class AppInsightsCounters : AbstractCounters
    {
        private readonly TelemetryClient _client;

        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Counters, "pip-services-runtime-counters", "app-insights", "*"
            );

        public AppInsightsCounters()
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

        protected override void Save(List<Counter> counters)
        {
            foreach (var counter in counters)
            {
                switch (counter.Type)
                {
                    case CounterType.Increment:
                        if (counter.Count != null)
                            _client.TrackMetric(counter.Name, counter.Count.Value);
                        break;
                    case CounterType.Interval:
                        if (counter.Avg != null)
                            _client.TrackMetric(counter.Name, counter.Avg.Value);
                        break;
                    case CounterType.LastValue:
                        if (counter.Last != null)
                            _client.TrackMetric(counter.Name, counter.Last.Value);
                        break;
                    case CounterType.Statistics:
                        if (counter.Avg != null)
                            _client.TrackMetric(counter.Name, counter.Avg.Value);
                        break;
                }
            }

            _client.Flush();
        }
    }
}
