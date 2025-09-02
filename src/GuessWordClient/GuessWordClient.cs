namespace GuessWordClient;

public class GuessWordClient(HttpClient httpClient) : IGuessWordClient {
	public async Task<MeInfo> GetMe(CancellationToken cancellationToken) {
		return await httpClient.GetFromJsonAsync<MeInfo>(Uris.Me, cancellationToken)
		       ?? throw new InvalidOperationException("Me is null");
	}

	public async Task<RoomInfo> CreateRoom(string name, RoomDifficulty difficulty, CancellationToken cancellationToken) {
		var request = new {
			name,
			difficulty,
			word = string.Empty
		};
		HttpResponseMessage response = await httpClient.PostAsJsonAsync(Uris.Rooms.AppendPathSegment("create/"), request, cancellationToken);
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<RoomInfo>(cancellationToken)
		       ?? throw new InvalidOperationException("Root was not created");
	}

	public async Task<IReadOnlyCollection<RoomInfo>> GetRooms(CancellationToken cancellationToken) {
		return await httpClient.GetFromJsonAsync<IReadOnlyCollection<RoomInfo>>(Uris.Rooms, cancellationToken)
		       ?? throw new InvalidOperationException("Rooms is null");
	}

	public async Task DeleteRoom(string slug, CancellationToken cancellationToken) {
		Url? uri = Uris.Rooms.AppendPathSegments(slug, '/');
		HttpResponseMessage response = await httpClient.DeleteAsync(uri, cancellationToken);
		response.EnsureSuccessStatusCode();
	}

	public async Task<GuessResult> Guess(string roomSlug, string word, CancellationToken cancellationToken) {
		Url? uri = Uris.Rooms.AppendPathSegments(roomSlug, "guess/");
		HttpResponseMessage response = await httpClient.PostAsJsonAsync(uri, new {
			word
		}, cancellationToken);
		if (response.StatusCode is HttpStatusCode.BadRequest) {
			ErrorResult errorResult = await response.Content.ReadFromJsonAsync<ErrorResult>(cancellationToken)
			                          ?? throw new InvalidOperationException("Error is null");
			throw new ErrorResultException(errorResult);
		}
		response.EnsureSuccessStatusCode();
		return await response.Content.ReadFromJsonAsync<GuessResult>(cancellationToken)
		       ?? throw new InvalidOperationException("Guess is null");
	}

	public async Task<RoomInfo> GetRoom(string slug, bool includeWord, CancellationToken cancellationToken) {
		Url? uri = Uris.Rooms
		               .AppendPathSegments(slug, '/')
		               .SetQueryParam("include_word", includeWord ? "true" : "false");

		return await httpClient.GetFromJsonAsync<RoomInfo>(uri, cancellationToken)
		       ?? throw new InvalidOperationException("Room is null");
	}

	private static class Uris {
		public const string Me = "api/me/";
		public const string Rooms = "api/rooms/";
	}
}
