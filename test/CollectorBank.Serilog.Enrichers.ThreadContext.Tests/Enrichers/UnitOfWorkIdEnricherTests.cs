using System;
using CollectorBank.Serilog.Enrichers.ThreadContext.Enrichers;
using CollectorBank.Serilog.Enrichers.ThreadContext.Tests.TestHelpers;
using NUnit.Framework;
using Serilog;
using Serilog.Events;

namespace CollectorBank.Serilog.Enrichers.ThreadContext.Tests.Enrichers
{

    public class UnitOfWorkIdEnricherTests
    {
        private const string UnitOfWorkIdName = "UnitOfWorkId";


        [TestCase("0CCC4AF2-A3D2-42E1-B807-0FDFBE458B05")]
        [TestCase("F77E0BC2-B58C-4447-93B9-2630B5466CE6")]
        public void Enrich_Update_LogEventPropertyUnitOfWorkIdSetToId(string id)
        {
            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.WithUnitOfWorkId()
                .WriteTo.Sink(new SerilogTestSink(e => evt = e))
                .CreateLogger();
            var guid = new Guid(id);

            UnitOfWorkIdEnricher.Update(guid);
            log.Information("Test");

            var actual = evt.GetPropertyValueAsString(UnitOfWorkIdName);
            Assert.That(actual, Is.EqualTo(guid.ToString()), $"{nameof(LogEvent)}.{nameof(evt.Properties)}[\"{UnitOfWorkIdName }\"]");
        }


        [TestCase("0CCC4AF2-A3D2-42E1-B807-0FDFBE458B05")]
        [TestCase("F77E0BC2-B58C-4447-93B9-2630B5466CE6")]
        public void Enrich_UpdateCalledPropertyAlreadySetByAnotherEnricher_LogEventPropertyUnitOfWorkIdSetToSameIdAsLastCallToUpdate(string id)
        {
            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.WithProperty(UnitOfWorkIdName, "EnricherA")
                .Enrich.WithUnitOfWorkId()
                .WriteTo.Sink(new SerilogTestSink(e => evt = e))
                .CreateLogger();

            UnitOfWorkIdEnricher.Update(new Guid(id));
            log.Information("Test");

            var actual = evt.GetPropertyValueAsString(UnitOfWorkIdName);
            Assert.That(actual, Is.EqualTo(new Guid(id).ToString()), $"{nameof(LogEvent)}.{nameof(evt.Properties)}[\"{UnitOfWorkIdName}\"]");
        }


        [TestCase("0CCC4AF2-A3D2-42E1-B807-0FDFBE458B05")]
        [TestCase("F77E0BC2-B58C-4447-93B9-2630B5466CE6")]
        public void Enrich_UpdateFollowedByClear_LogEventPropertyUnitOfWorkIdIsdNull(string id)
        {
            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.WithUnitOfWorkId()
                .WriteTo.Sink(new SerilogTestSink(e => evt = e))
                .CreateLogger();
            var guid = new Guid(id);

            UnitOfWorkIdEnricher.Update(guid);
            UnitOfWorkIdEnricher.Clear();
            log.Information("Test");

            var actual = evt.GetPropertyValueAsString(UnitOfWorkIdName);
            Assert.That(actual, Is.Null, $"{nameof(LogEvent)}.{nameof(evt.Properties)}[\"{UnitOfWorkIdName}\"]");
        }
    }
}