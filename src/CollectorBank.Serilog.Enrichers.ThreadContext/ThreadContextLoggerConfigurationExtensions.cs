using System;
using CollectorBank.Serilog.Enrichers.ThreadContext.Enrichers;
using Serilog;
using Serilog.Configuration;

namespace CollectorBank.Serilog.Enrichers.ThreadContext
{


    /// <summary>Extends <see cref="LoggerConfiguration"/> to add enrichers for correlation id and unit of work id associated with a thread.</summary>
    public static class ThreadContextLoggerConfigurationExtensions
    {

        /// <summary>Enrich log events with a CorrelationId property containing the correlation id associated with current thread.</summary>
        /// <param name="enrichmentConfiguration">Logger enrichment configuration.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration WithCorrelationId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
                throw new ArgumentNullException(nameof(enrichmentConfiguration));

            return enrichmentConfiguration.With<CorrelationIdEnricher>();
        }


        /// <summary>Enrich log events with a UnitOfWorkId property containing the unit of work id associated with current thread.</summary>
        /// <param name="enrichmentConfiguration">Logger enrichment configuration.</param>
        /// <returns>Configuration object allowing method chaining.</returns>
        public static LoggerConfiguration WithUnitOfWorkId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            if (enrichmentConfiguration == null)
                throw new ArgumentNullException(nameof(enrichmentConfiguration));

            return enrichmentConfiguration.With<UnitOfWorkIdEnricher>();
        }
    }
}