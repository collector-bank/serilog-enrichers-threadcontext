using System;
using System.Threading;
using Serilog.Core;
using Serilog.Events;

namespace CollectorBank.Serilog.Enrichers.ThreadContext.Enrichers
{

    /// <summary>
    /// Enricher that can associate a UnitOfWorkId with the current thread.
    /// Use <see cref="Update"/> to update the UnitOfWorkId for the current thread and <see cref="Clear"/> to clear it.
    /// </summary>
    public class UnitOfWorkIdEnricher : ILogEventEnricher
    {
        private const string UnitOfWorkIdPropertyName = "UnitOfWorkId";
        private static readonly AsyncLocal<Guid> UnitOfWorkIdContext = new AsyncLocal<Guid>();


        /// <summary>Update the Unit of Work Id associated with the current thread context.</summary>
        /// <param name="guid">The unit of work id.</param>
        public static void Update(Guid guid)
        {
            UnitOfWorkIdContext.Value = guid;
        }


        /// <summary>Clear the Unit of Work Id  associated with the current thread context.</summary>
        public static void Clear()
        {
            UnitOfWorkIdContext.Value = Guid.Empty;
        }


        /// <summary>Enrich the log event.</summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">Factory for creating new properties to add to the event.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var unitOfWorkId = UnitOfWorkIdContext.Value;
            if (unitOfWorkId != Guid.Empty)
            {
                logEvent.AddOrUpdateProperty(new LogEventProperty(UnitOfWorkIdPropertyName, new ScalarValue(unitOfWorkId)));
            }
        }
    }
}