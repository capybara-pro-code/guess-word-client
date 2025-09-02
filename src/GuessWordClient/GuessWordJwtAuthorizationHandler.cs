using System.IdentityModel.Tokens.Jwt;

namespace GuessWordClient;

public class GuessWordJwtAuthorizationHandler : DelegatingHandler {
	private readonly HttpClient _httpClient;
	private readonly IOptions<GuessWordClientOptions> _options;

	private readonly SemaphoreSlim _semaphore = new(1);

	private JwtToken? _jwt;

	public GuessWordJwtAuthorizationHandler(IOptions<GuessWordClientOptions> options, HttpClient httpClient) {
		_options = options;
		_httpClient = httpClient;
	}

	private string JwtFilePath {
		get {
			string credentialHash = BitConverter
			                        .ToString(SHA1.HashData(Encoding.UTF8.GetBytes($"{_options.Value.Username}:{_options.Value.Password}")))
			                        .Replace("-", string.Empty);
			return Path.Join(Directory.GetCurrentDirectory(), $"jwt.{credentialHash}.json");
		}
	}

	private JwtToken? Jwt {
		get {
			_jwt ??= File.Exists(JwtFilePath) ? JsonSerializer.Deserialize<JwtToken>(File.OpenRead(JwtFilePath)) : null;
			return _jwt;
		}
		set {
			_jwt = value;
			File.WriteAllBytes(JwtFilePath, JsonSerializer.SerializeToUtf8Bytes(_jwt));
		}
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
	                                                             CancellationToken cancellationToken) {
		await _semaphore.WaitAsync(cancellationToken);
		JwtToken jwt = await GetOrCreateJwt(cancellationToken);
		try {
			var handler = new JwtSecurityTokenHandler();
			JwtSecurityToken? jwtSecurityToken = handler.ReadJwtToken(jwt.Access);
			if (jwtSecurityToken.ValidTo < DateTime.UtcNow.AddMinutes(5)) {
				jwt = await RefreshJwtToken(cancellationToken);
			}
		} finally {
			_semaphore.Release();
		}

		request.Headers.Authorization = new AuthenticationHeaderValue("JWT", jwt.Access);

		return await base.SendAsync(request, cancellationToken);
	}

	private async Task<JwtToken> GetOrCreateJwt(CancellationToken cancellationToken) {
		return Jwt ??= await CreateJwt();

		async Task<JwtToken> CreateJwt() {
			var auth = new {
				username = _options.Value.Username,
				password = _options.Value.Password
			};
			Url? uri = _options.Value.BaseUri.AppendPathSegments(Uris.Auth, "jwt", "create");
			HttpResponseMessage response = await _httpClient.PostAsJsonAsync(uri, auth, cancellationToken);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<JwtToken>(cancellationToken) ??
			       throw new InvalidOperationException("Jwt is null");
		}
	}

	private async Task<JwtToken> RefreshJwtToken(CancellationToken cancellationToken) {
		return Jwt = await RefreshJwt();

		async Task<JwtToken> RefreshJwt() {
			var refresh = new {
				refresh = Jwt?.Refresh ?? throw new InvalidOperationException("JWT is null")
			};
			Url? uri = _options.Value.BaseUri.AppendPathSegments(Uris.Auth, "jwt", "refresh");
			HttpResponseMessage response = await _httpClient.PostAsJsonAsync(uri, refresh, cancellationToken);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadFromJsonAsync<JwtToken>(cancellationToken) ??
			       throw new InvalidOperationException("Jwt is null");
		}
	}

	private static class Uris {
		public const string Auth = "api/auth";
	}

	// ReSharper disable once ClassNeverInstantiated.Local
	private record JwtToken(string Access, string Refresh);
}
