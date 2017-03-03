using System;
using System.Threading.Tasks;
using CollectorBank.Serilog.Enrichers.ThreadContext.Enrichers;
using Serilog;

namespace CollectorBank.Serilog.Enrichers.ThreadContext.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithUnitOfWorkId()
                .Enrich.WithCorrelationId()
                .WriteTo.Sink(new TestFileBatchingSink("test.txt", 100, new TimeSpan(0, 0, 0, 15)))
                .CreateLogger();

            Log.Logger.Information("Integration Test ... Starting");
            var taskA = Task.Run(() =>
            {
                for (var i = 0; i < 100; ++i)
                {
                    System.Threading.Thread.Sleep(500);
                    CorrelationIdEnricher.Update(Guid.NewGuid());
                    UnitOfWorkIdEnricher.Update(Guid.NewGuid());
                    System.Threading.Thread.Sleep(500);
                    Log.Information("Task A: creates new ids");
                }
            });

            var taskB = Task.Run(() =>
            {
                var correlationIds = new[] {Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()};
                for (var i = 0; i < 100; ++i)
                {
                    System.Threading.Thread.Sleep(450);
                    var corrIndex = i % correlationIds.Length;
                    CorrelationIdEnricher.Update(correlationIds[corrIndex]);
                    UnitOfWorkIdEnricher.Update(Guid.NewGuid());

                    System.Threading.Thread.Sleep(450);
                    Log.Information($"Task B: correlation id index {corrIndex}");
                }
            });

            var taskC = Task.Run(() =>
            {
                CorrelationIdEnricher.Update(Guid.NewGuid());
                UnitOfWorkIdEnricher.Update(Guid.NewGuid());
                for (var i = 0; i < 100; ++i)
                {
                    Log.Information($"Task C: long running wiht same ids");
                    System.Threading.Thread.Sleep(1000);
                }
            });

            Log.Logger.Information("Waiting for task to finnish");
            Task.WaitAll(taskA, taskB, taskC);
            Log.Logger.Information("Integration Test ... Completed");
        }
    }
}