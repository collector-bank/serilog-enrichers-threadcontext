# CollectorBank.Serilog.Enrichers.ThreadContext  

Enrichers that enrich Serilog events with CorrelationId and UnitOfWorkId based on the current thread task context.

---
## CorrelationId vs UnitOfWorkId
CorrelationId can be sent by external system will the idea of UnitOfWorkId is to set an id when doing work.  
For example, an external system can make a HTTP request with a CorrelationId and for the request a UnitOfWorkId is set.  
The external system might get an error back and later will make the exact same request then the CorrelationId will be  
the same will the UnitOfWorkId will be different between the request.

The same might apply when processing message from a service bus where the message has a CorrelationId  and the processing of the message get a UnitOfWorkId.

## Included enrichers

The package includes:

 * `WithCorrelationId` - adds the CorrelationId associated with the current thread task context to the log event.
 * `WithUnitOfWorkId` - adds the UnitOfWorkId associated with the current thread task context to the log event.

---
## Setup Guide

To use the enrichers, first install the NuGet package:

```powershell
Install-Package CollectorBank.Serilog.Enrichers.ThreadContext
```

Then apply the enrichers to your `LoggerConfiguration`:

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.WithCorrelationId()
    .Enrich.WithUnitOfWorkId()
    .CreateLogger();
```

And then you need to set the CorrelationId or UnitOfWorkId for the current thread task context.  
You can clear them by calling Clear or by calling Update with Guid.Empty then the ids will not be added to the log event.

```csharp
    void ProcessMessage(Message msg) {
        try
        {
            CorrelationIdEnricher.Update(msg.CorrelationId);
            UnitOfWorkIdEnricher.Update(Guid.NewGuid());
            // Process message
        }
        finally {
            CorrelationIdEnricher.Clear();
            UnitOfWorkIdEnricher.Clear();
        }
    }
```
