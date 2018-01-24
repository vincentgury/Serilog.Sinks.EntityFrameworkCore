namespace VG.Serilog.Sinks.EntityFrameworkCore
{
    using System;

    using global::Serilog;
    using global::Serilog.Configuration;

    public static class EntityFrameworkCoreSinkExtensions
    {
        public static LoggerConfiguration EntityFrameworkSink(
                  this LoggerSinkConfiguration loggerConfiguration,
                  Func<LogDbContext> dbContextProvider,
                  IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new EntityFrameworkCoreSink(dbContextProvider, formatProvider));
        }
    }
}
