namespace GuessWordClient;

public static class ServiceCollectionExtensions {
	public static IHttpClientBuilder AddGuessWordClient(this IServiceCollection services, Action<GuessWordClientOptions>? configureOptions = null) {
		if (configureOptions != null) {
			services.Configure(configureOptions);
		}
		return services.AddSingleton<GuessWordJwtAuthorizationHandler>()
		               .AddHttpClient<IGuessWordClient, GuessWordClient>((sp, client) => {
			               string baseUri = sp.GetRequiredService<IOptions<GuessWordClientOptions>>().Value.BaseUri;
			               client.BaseAddress = new Uri(baseUri);
		               })
		               .AddHttpMessageHandler<GuessWordJwtAuthorizationHandler>();
	}

	public static IHttpClientBuilder AddGuessWordClient(this IServiceCollection services, Action<IServiceProvider, GuessWordClientOptions>? configureOptions) {
		if (configureOptions != null) {
			services.AddOptions();
			services.AddSingleton<IConfigureOptions<GuessWordClientOptions>>(
				provider => new ConfigureOptions<GuessWordClientOptions>(options => configureOptions(provider, options)));
		}
		return services.AddSingleton<GuessWordJwtAuthorizationHandler>()
		               .AddHttpClient<IGuessWordClient, GuessWordClient>((sp, client) => {
			               string baseUri = sp.GetRequiredService<IOptions<GuessWordClientOptions>>().Value.BaseUri;
			               client.BaseAddress = new Uri(baseUri);
		               })
		               .AddHttpMessageHandler<GuessWordJwtAuthorizationHandler>();
	}
}
