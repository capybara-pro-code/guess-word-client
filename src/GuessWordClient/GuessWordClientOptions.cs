namespace GuessWordClient;

public record GuessWordClientOptions {
	public string BaseUri { get; init; } = "https://guess-word.com/";

#if NET8_0_OR_GREATER
	public required string Username { get; init; }
	public required string Password { get; init; }
#else
	public string Username { get; init; } = null!;
	public string Password { get; init; } = null!;
#endif
}
