namespace VG.Serilog.Sinks.EntityFrameworkCore
{
    using Microsoft.EntityFrameworkCore;

    public class LogDbContext : DbContext
    {
        public DbSet<LogRecord> LogRecords { get; set; }

        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {
            
        }
    }
}
