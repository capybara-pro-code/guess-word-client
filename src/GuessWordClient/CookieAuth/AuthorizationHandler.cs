namespace GuessWordClient.CookieAuth;

public class AuthorizationHandler(IOptions<GuessWordClientOptions> options, IHttpClientFactory httpClientFactory) : DelegatingHandler {
	private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(AuthorizationHandler));

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
	                                                             CancellationToken cancellationToken) {
		HttpResponseMessage response;
		if (request.Headers.Contains("X-CSRFToken")) {
			response = await base.SendAsync(request, cancellationToken);
			if (response.StatusCode is not (HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)) {
				return response;
			}
		} else {
			response = await _httpClient.GetAsync("api/me/", cancellationToken);
			if (response.StatusCode is not (HttpStatusCode.Forbidden or HttpStatusCode.Unauthorized)) {
				return await base.SendAsync(request, cancellationToken);
			}
		}
		HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync(Uris.Login, new {
			username = options.Value.Username,
			password = options.Value.Password
		}, cancellationToken: cancellationToken);
		responseMessage.EnsureSuccessStatusCode();
		string result = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
		if (result == """
		              "Auth success"
		              """) {
			return await base.SendAsync(request, cancellationToken);
		} else {
			throw new HttpRequestException("Authorization failed", null, HttpStatusCode.Unauthorized);
		}
	}

	private static class Uris {
		public const string Login = "api/auth/login/";
	}
}
