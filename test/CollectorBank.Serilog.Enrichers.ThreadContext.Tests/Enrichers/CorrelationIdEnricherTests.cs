using System;
using CollectorBank.Serilog.Enrichers.ThreadContext.Enrichers;
using CollectorBank.Serilog.Enrichers.ThreadContext.Tests.TestHelpers;
using NUnit.Framework;
using Serilog;
using Serilog.Events;

namespace CollectorBank.Serilog.Enrichers.ThreadContext.Tests.Enrichers
{

    public class CorrelationIdEnricherTests
    {
        private const string CorrelationIdName = "CorrelationId";


        [TestCase("0CCC4AF2-A3D2-42E1-B807-0FDFBE458B05")]
        [TestCase("F77E0BC2-B58C-4447-93B9-2630B5466CE6")]
        public void Enrich_Update_LogEventPropertyCorrelationIdSetToId(string id)
        {
            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.WithCorrelationId()
                .WriteTo.Sink(new SerilogTestSink(e => evt = e))
                .CreateLogger();
            var guid = new Guid(id);

            CorrelationIdEnricher.Update(guid);
            log.Information("Test");

            var actual = evt.GetPropertyValueAsString(CorrelationIdName);
            Assert.That(actual, Is.EqualTo(guid.ToString()), $"{nameof(LogEvent)}.{nameof(evt.Properties)}[\"{CorrelationIdName}\"]");
        }


        [TestCase("0CCC4AF2-A3D2-42E1-B807-0FDFBE458B05")]
        [TestCase("F77E0BC2-B58C-4447-93B9-2630B5466CE6")]
        public void Enrich_UpdateCalledPropertyAlreadySetByAnotherEnricher_LogEventPropertyCorrelationIdSetToSameIdAsLastCallToUpdate(string id)
        {
            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.WithProperty(CorrelationIdName, "EnricherA")
                .Enrich.WithCorrelationId()
                .WriteTo.Sink(new SerilogTestSink(e => evt = e))
                .CreateLogger();

            CorrelationIdEnricher.Update(new Guid(id));
            log.Information("Test");

            var actual = evt.GetPropertyValueAsString(CorrelationIdName);
            Assert.That(actual, Is.EqualTo(new Guid(id).ToString()), $"{nameof(LogEvent)}.{nameof(evt.Properties)}[\"{CorrelationIdName}\"]");
        }


        [TestCase("0CCC4AF2-A3D2-42E1-B807-0FDFBE458B05")]
        [TestCase("F77E0BC2-B58C-4447-93B9-2630B5466CE6")]
        public void Enrich_UpdateFollowedByClear_LogEventPropertyCorrelationIdIsNull(string id)
        {
            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.WithCorrelationId()
                .WriteTo.Sink(new SerilogTestSink(e => evt = e))
                .CreateLogger();
            var guid = new Guid(id);

            CorrelationIdEnricher.Update(guid);
            CorrelationIdEnricher.Clear();
            log.Information("Test");

            var actual = evt.GetPropertyValueAsString(CorrelationIdName);
            Assert.That(actual, Is.Null, $"{nameof(LogEvent)}.{nameof(evt.Properties)}[\"{CorrelationIdName}\"]");
        }
    }
}