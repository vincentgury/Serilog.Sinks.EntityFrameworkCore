namespace VG.Serilog.Sinks.EntityFrameworkCore
{
    using System;
    using System.IO;
    using System.Text;

    using global::Serilog.Core;
    using global::Serilog.Events;
    using global::Serilog.Formatting.Json;

    using Microsoft.EntityFrameworkCore;

    using Newtonsoft.Json.Linq;

    public class EntityFrameworkCoreSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly Func<DbContext> _dbContextProvider;
        private readonly JsonFormatter _jsonFormatter;
        static readonly object _lock = new object();

        public EntityFrameworkCoreSink(Func<DbContext> dbContextProvider, IFormatProvider formatProvider)
        {
            this._formatProvider = formatProvider;
            this._dbContextProvider = dbContextProvider ?? throw new ArgumentNullException(nameof(dbContextProvider));
            this._jsonFormatter = new JsonFormatter(formatProvider: formatProvider);
        }

        public void Emit(LogEvent logEvent)
        {
            //lock (_lock)
            //{
                if (logEvent == null)
                {
                    return;
                }

                try
                {
                    DbContext context = this._dbContextProvider.Invoke();

                    if (context != null)
                    {
                        context.Set<LogRecord>().AddAsync(this.ConvertLogEventToLogRecord(logEvent));

                        context.SaveChangesAsync();
                    }
                }
                catch
                {
                    // ignored
                }
            //}
        }

        private LogRecord ConvertLogEventToLogRecord(LogEvent logEvent)
        {
            if (logEvent == null)
            {
                return null;
            }

            string json = this.ConvertLogEventToJson(logEvent);

            JObject jObject = JObject.Parse(json);
            JToken properties = jObject["Properties"];

            return new LogRecord
            {
                Exception = logEvent.Exception?.ToString(),
                Level = logEvent.Level.ToString(),
                LogEvent = json,
                Message = this._formatProvider == null ? null : logEvent.RenderMessage(this._formatProvider),
                MessageTemplate = logEvent.MessageTemplate?.ToString(),
                TimeStamp = logEvent.Timestamp.DateTime.ToUniversalTime(),
                EventId = (int?) properties["EventId"]?["Id"],
                SourceContext = (string) properties["SourceContext"],
                ActionId = (string) properties["ActionId"],
                ActionName = (string) properties["ActionName"],
                RequestId = (string) properties["RequestId"],
                RequestPath = (string) properties["RequestPath"]
            };
        }

        private string ConvertLogEventToJson(LogEvent logEvent)
        {
            if (logEvent == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                this._jsonFormatter.Format(logEvent, writer);
            }

            return sb.ToString();
        }
    }
}