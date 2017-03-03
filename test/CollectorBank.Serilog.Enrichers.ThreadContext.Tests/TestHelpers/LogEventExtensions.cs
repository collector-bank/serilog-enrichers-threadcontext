using Serilog.Events;

namespace CollectorBank.Serilog.Enrichers.ThreadContext.Tests.TestHelpers
{

    public static class LogEventExtensions
    {

        public static string GetPropertyValueAsString(this LogEvent evt, string key)
        {
            LogEventPropertyValue propValue;
            if (evt.Properties.TryGetValue(key, out propValue))
                return propValue?.ToString();

            return null;
        }
    }
}