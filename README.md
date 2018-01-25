# Serilog.Sinks.EntityFrameworkCore
A Serilog sink for Entity Framework Core

### Important
This is not an official Serilog Sink. No support provided. Use it at your own risk.

### Getting started

To use the Entity Framework Core sink, first install the [NuGet package](https://www.nuget.org/packages/VG.Serilog.Sinks.EntityFrameworkCore). The package must be installed in both Web project and where your migrations files are saved:

```powershell
Install-Package VG.Serilog.Sinks.EntityFrameworkCore
```

Then register the LogDbCOntext and enable the sink (feel free to adapt the following code if you want to use a separate database or move the migrations to another assembly):

```csharp
public void ConfigureServices(IServiceCollection services)
{
	string migrationAssembly = typeof(YourDbContext).GetTypeInfo().Assembly.GetName().Name;

	services.AddDbContext<YourDbContext>(opt => opt.UseSqlServer(this.Configuration.GetConnectionString("DefaultServerConnection")));
	services.AddDbContext<LogDbContext>(options => options.UseSqlServer(this.Configuration.GetConnectionString("DefaultServerConnection"), b => b.MigrationsAssembly(migrationAssembly)));
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
	Log.Logger = new LoggerConfiguration()
        .WriteTo.EntityFrameworkSink(app.ApplicationServices.GetService<LogDbContext>)
        .CreateLogger();
		
	loggerFactory.AddSerilog();
}
```

Add migrations and update database

```powershell
Add-Migration InitLog -Context LogDbContext -OutputDir Migrations/LogDb
Add-Migration InitYourDb -Context YourDbContext -OutputDir Migrations/YourDb
Update-Database -Context LogDbContext
Update-Database -Context YourDbContext
```

### Usage

```csharp
Log.Information("Hello, world!");
```
