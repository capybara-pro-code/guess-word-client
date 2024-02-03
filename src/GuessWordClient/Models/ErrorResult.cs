namespace GuessWordClient.Models;

public record ErrorResult(
	[property: JsonPropertyName("detail")]
	string Detail,
	[property: JsonPropertyName("error_code")]
	ErrorCode ErrorCode
);
