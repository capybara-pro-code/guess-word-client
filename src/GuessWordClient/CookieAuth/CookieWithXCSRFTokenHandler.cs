namespace GuessWordClient.CookieAuth;

public class CookieWithXcsrfTokenHandler : HttpClientHandler {
	private readonly string? _cookieFilePath;
	private readonly SemaphoreSlim _semaphore = new(1);

	private readonly JsonSerializerOptions _jsonSerializerOptions = new() {
		WriteIndented = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	};

	public CookieWithXcsrfTokenHandler(IOptions<GuessWordClientOptions> options) {
		_cookieFilePath = options.Value.StoreCookieInFile ? options.Value.CookieFilePath : null;
		CookieContainer = LoadOrCreateCookie();
		UseCookies = true;
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
		await _semaphore.WaitAsync(cancellationToken);
		try {
			string? csrftoken = CookieContainer.GetAllCookies().FirstOrDefault(cookie => cookie.Name == "csrftoken")?.Value;
			if (csrftoken != null) {
				request.Headers.Add("X-CSRFToken", csrftoken);
			}
			HttpResponseMessage result = await base.SendAsync(request, cancellationToken);
			if (_cookieFilePath is not null && result.IsSuccessStatusCode) {
				SaveCookie(CookieContainer.GetAllCookies());
			}
			return result;
		} finally {
			_semaphore.Release();
		}
	}

	private void SaveCookie(CookieCollection cookieCollection) {
		var cookies = new List<SerializableCookie>();
		foreach (Cookie cookie in cookieCollection) {
			cookies.Add(new SerializableCookie(cookie));
		}
		string json = JsonSerializer.Serialize(cookies, _jsonSerializerOptions);
		File.WriteAllText(_cookieFilePath ?? throw new InvalidOperationException(), json);
	}

	private CookieContainer LoadOrCreateCookie() {
		if (_cookieFilePath is null || !File.Exists(_cookieFilePath)) {
			return new CookieContainer();
		}
		string json = File.ReadAllText(_cookieFilePath);
		var cookies = JsonSerializer.Deserialize<List<SerializableCookie>>(json)!;
		var cookieContainer = new CookieContainer();
		foreach (SerializableCookie cookie in cookies) {
			cookieContainer.Add(cookie.ToCookie());
		}
		return cookieContainer;
	}
}
