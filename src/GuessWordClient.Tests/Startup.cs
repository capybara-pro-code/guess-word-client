namespace GuessWordClient.Tests;

[PublicAPI]
internal class Startup {
	public static void ConfigureHost(IHostBuilder hostBuilder) {
		hostBuilder
			.ConfigureLogging(builder => {
				builder.SetMinimumLevel(LogLevel.Trace);
				builder.AddXunitOutput();
			})
			.ConfigureHostConfiguration(builder => {
				builder.AddUserSecrets(Assembly.GetExecutingAssembly());
				builder.AddEnvironmentVariables();
			})
			.ConfigureServices((context, services) => {
				services.AddSingleton<TestsValues>(sp =>
					                                   new TestsValues(sp.GetRequiredService<IConfiguration>().GetValue<string>("RoomsPrefix") ?? $"{Environment.UserName}@{Environment.MachineName}"));
				services.AddHostedService<CleanupService>();
				services.Configure<GuessWordClientOptions>(context.Configuration.GetSection("GuessWordClient"));
				services.AddGuessWordClient(options => options.StoreCookieInFile = false);
			});
	}
}
