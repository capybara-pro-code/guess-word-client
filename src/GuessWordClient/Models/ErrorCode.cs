namespace GuessWordClient.Models;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
[JsonStringEnumMemberConverterOptions(deserializationFailureFallbackValue: Unknown)]
public enum ErrorCode {
	Unknown,

	[JsonPropertyName("word_not_found")]
	WordNotFound
}
