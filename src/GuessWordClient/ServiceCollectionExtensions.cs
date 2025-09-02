using GuessWordClient.CookieAuth;

namespace GuessWordClient;

public static class ServiceCollectionExtensions {
	public static IServiceCollection AddGuessWordClient(this IServiceCollection services, Action<GuessWordClientOptions>? configureOptions = null) {
		if (configureOptions != null) {
			services.Configure(configureOptions);
		}
		services
			.AddSingleton<CookieWithXcsrfTokenHandler>()
			.AddSingleton<AuthorizationHandler>();

		services
			.AddHttpClient<IGuessWordClient, GuessWordClient>((sp, client) => {
				string baseUri = sp.GetRequiredService<IOptions<GuessWordClientOptions>>().Value.BaseUri;
				client.BaseAddress = new Uri(baseUri);
			})
			.ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<CookieWithXcsrfTokenHandler>())
			.AddHttpMessageHandler<AuthorizationHandler>();

		services
			.AddHttpClient(nameof(AuthorizationHandler), (sp, client) => {
				string baseUri = sp.GetRequiredService<IOptions<GuessWordClientOptions>>().Value.BaseUri;
				client.BaseAddress = new Uri(baseUri);
			})
			.ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<CookieWithXcsrfTokenHandler>());

		return services;
	}

	public static IServiceCollection AddGuessWordClient(this IServiceCollection services, Action<IServiceProvider, GuessWordClientOptions>? configureOptionsWithServiceProvider) {
		if (configureOptionsWithServiceProvider == null) {
			return AddGuessWordClient(services, configureOptions: null);
		}
		services.AddOptions();
		services.AddSingleton<IConfigureOptions<GuessWordClientOptions>>(provider => new ConfigureOptions<GuessWordClientOptions>(options => configureOptionsWithServiceProvider(provider, options)));
		return AddGuessWordClient(services, configureOptions: null);
	}
}
