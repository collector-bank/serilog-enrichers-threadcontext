using System;
using System.Threading;
using Serilog.Core;
using Serilog.Events;

namespace CollectorBank.Serilog.Enrichers.ThreadContext.Enrichers
{

    /// <summary>
    /// Enricher that can associate a CorrelationId with the current thread.
    /// Use <see cref="Update"/> to update the CorrelationId for the current thread and <see cref="Clear"/> to clear it.
    /// </summary>
    public class CorrelationIdEnricher : ILogEventEnricher
    {
        private const string CorrelationIdPropertyName = "CorrelationId";
        private static readonly AsyncLocal<Guid> CorrelationIdContext = new AsyncLocal<Guid>();


        /// <summary>Update the Correlation Id associated with the current thread context.</summary>
        /// <param name="guid">The correlation id.</param>
        public static void Update(Guid guid)
        {
            CorrelationIdContext.Value = guid;
        }


        /// <summary>Clear the Correlation Id associated with the current thread context.</summary>
        public static void Clear()
        {
            CorrelationIdContext.Value = Guid.Empty;
        }


        /// <summary>Enrich the log event.</summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var correlationId = CorrelationIdContext.Value;
            if (correlationId != Guid.Empty)
            {
                logEvent.AddOrUpdateProperty(new LogEventProperty(CorrelationIdPropertyName, new ScalarValue(correlationId) ));
            }
        }
    }
}