namespace GuessWordClient;

public record GuessWordClientOptions {
	public string BaseUri { get; set; } = "https://guess-word.com/";

	public required string Username { get; set; }
	public required string Password { get; set; }
	public required bool StoreCookieInFile { get; set; } = true;
	public required string CookieFilePath { get; set; } = "cookie.json";
}
