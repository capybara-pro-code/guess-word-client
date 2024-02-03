using System.Reflection;
using Xunit.DependencyInjection.Logging;

namespace GuessWordClient.Tests;

public class Startup {
	public void ConfigureHost(IHostBuilder hostBuilder) {
		hostBuilder
			.ConfigureLogging(builder => {
				builder.SetMinimumLevel(LogLevel.Trace);
				builder.AddXunitOutput();
			})
			.ConfigureHostConfiguration(builder => {
				builder.AddUserSecrets(Assembly.GetExecutingAssembly());
			})
			.ConfigureServices((context, services) => {
				services.AddHostedService<CleanupService>();
				services.Configure<GuessWordClientOptions>(context.Configuration.GetSection("GuessWordClient"));
				services.AddGuessWordClient();
			});
	}
}
