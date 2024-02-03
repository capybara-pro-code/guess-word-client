namespace GuessWordClient.Models;

[PublicAPI]
public record GuessResult(
	[property: JsonPropertyName("order")]
	uint Order,
	[property: JsonPropertyName("already_guessed")]
	bool AlreadyGuessed
);
