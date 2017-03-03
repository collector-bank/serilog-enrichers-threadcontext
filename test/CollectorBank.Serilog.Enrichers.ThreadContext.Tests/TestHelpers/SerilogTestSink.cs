using System;
using Serilog.Core;
using Serilog.Events;

namespace CollectorBank.Serilog.Enrichers.ThreadContext.Tests.TestHelpers
{
    public class SerilogTestSink : ILogEventSink
    {

        readonly Action<LogEvent> _write;


        public SerilogTestSink(Action<LogEvent> write)
        {
            if (write == null)
                throw new ArgumentNullException(nameof(write));

            _write = write;
        }


        public void Emit(LogEvent logEvent)
        {
            _write(logEvent);
        }
    }
}