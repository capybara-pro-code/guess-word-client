namespace GuessWordClient.Models;

public record MeInfo(
	[property: JsonPropertyName("username")]
	string Username
);
