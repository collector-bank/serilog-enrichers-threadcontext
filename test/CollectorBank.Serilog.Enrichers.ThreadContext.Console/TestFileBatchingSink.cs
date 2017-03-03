using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Sinks.PeriodicBatching;

namespace CollectorBank.Serilog.Enrichers.ThreadContext.Console
{

    public class TestFileBatchingSink : PeriodicBatchingSink
    {
        private readonly string _path;

        public TestFileBatchingSink(string path, int batchSizeLimit, TimeSpan period)
            : base(batchSizeLimit, period)
        {
            _path = path;
            
        }


        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            var sb = new StringBuilder();
            var jsonFormatter = new JsonFormatter();
            
            foreach (var logEvent in events)
            {
                using (var render = new StringWriter())
                {
                    jsonFormatter.Format(logEvent, render);
                    sb.AppendLine(render.ToString());
                }
            }

            using (var fileStream = new FileStream(_path, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write))
            using (var streamWriter = new StreamWriter(fileStream))
            {
                await streamWriter.WriteAsync(sb.ToString());
            }
        }
    }
}