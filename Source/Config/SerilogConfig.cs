namespace App;

using Serilog;
using Serilog.Events;

public static class SerilogConfig {
	/// We use Serilog for file logging.
	/// The log files are located inside project, for eg,. under `./logs/` folder.
	/// Ref: https://github.com/serilog/serilog-aspnetcore/blob/dev/samples/Sample/Program.cs
	public static void ConfigureSerilogDk(this WebApplicationBuilder builder) {
		builder.Host.UseSerilog((context, services, configuration) => configuration
			// .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
			// .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Information)
			// .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Information)
			.ReadFrom.Configuration(context.Configuration)
			.ReadFrom.Services(services)
			.Enrich.FromLogContext()

			// Log to console (useful for development)
			.WriteTo.Console()

			// Log per level to file
			.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.File(
				path: Path.Combine("logs", "debug-.txt"), // File name: log-202210_001.txt
				fileSizeLimitBytes: 100 * 1024 * 1024, // 100 MB/file
				rollingInterval: RollingInterval.Month, // Every month
				retainedFileCountLimit: 100, // -> Total log size: 100 * 100 MB = 10 GB
				rollOnFileSizeLimit: true, // New file with suffix _* will be created when filesize limit is reached
				shared: true, // Available for another process
				flushToDiskInterval: TimeSpan.FromSeconds(30) // Flush log to disk each 30s
			))
			.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.File(
				path: Path.Combine("logs", "info-.txt"), // File name: log-202210_001.txt
				fileSizeLimitBytes: 100 * 1024 * 1024, // 100 MB/file
				rollingInterval: RollingInterval.Month, // Every month
				retainedFileCountLimit: 100, // -> Total log size: 100 * 100 MB = 10 GB
				rollOnFileSizeLimit: true, // New file with suffix _* will be created when filesize limit is reached
				shared: true, // Available for another process
				flushToDiskInterval: TimeSpan.FromSeconds(30) // Flush log to disk each 30s
			))
			.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo.File(
				path: Path.Combine("logs", "warning-.txt"), // File name: log-202210_001.txt
				fileSizeLimitBytes: 100 * 1024 * 1024, // 100 MB/file
				rollingInterval: RollingInterval.Month, // Every month
				retainedFileCountLimit: 100, // -> Total log size: 100 * 100 MB = 10 GB
				rollOnFileSizeLimit: true, // New file with suffix _* will be created when filesize limit is reached
				shared: true, // Available for another process
				flushToDiskInterval: TimeSpan.FromSeconds(30) // Flush log to disk each 30s
			))
			.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.File(
				path: Path.Combine("logs", "error-.txt"), // File name: log-202210_001.txt
				fileSizeLimitBytes: 100 * 1024 * 1024, // 100 MB/file
				rollingInterval: RollingInterval.Month, // Every month
				retainedFileCountLimit: 100, // -> Total log size: 100 * 100 MB = 10 GB
				rollOnFileSizeLimit: true, // New file with suffix _* will be created when filesize limit is reached
				shared: true, // Available for another process
				flushToDiskInterval: TimeSpan.FromSeconds(30) // Flush log to disk each 30s
			))
			.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal).WriteTo.File(
				path: Path.Combine("logs", "critical-.txt"), // File name: log-202210_001.txt
				fileSizeLimitBytes: 100 * 1024 * 1024, // 100 MB/file
				rollingInterval: RollingInterval.Month, // Every month
				retainedFileCountLimit: 100, // -> Total log size: 100 * 100 MB = 10 GB
				rollOnFileSizeLimit: true, // New file with suffix _* will be created when filesize limit is reached
				shared: true, // Available for another process
				flushToDiskInterval: TimeSpan.FromSeconds(30) // Flush log to disk each 30s
			))
			// // For another log types, we log as verbose.
			// .WriteTo.File(
			// 	path: Path.Combine("logs", "verbose-.txt"), // File name: log-202210_001.txt
			// 	fileSizeLimitBytes: 100 * 1024 * 1024, // 100 MB/file
			// 	rollingInterval: RollingInterval.Month, // Every month
			// 	retainedFileCountLimit: 100, // -> Total log size: 100 * 100 MB = 10 GB
			// 	rollOnFileSizeLimit: true, // New file with suffix _* will be created when filesize limit is reached
			// 	shared: true, // Available for another process
			// 	flushToDiskInterval: TimeSpan.FromSeconds(30) // Flush log to disk each 30s
			// )
		);
	}
}
