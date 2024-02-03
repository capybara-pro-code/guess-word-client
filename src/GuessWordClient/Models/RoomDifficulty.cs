namespace GuessWordClient.Models;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum RoomDifficulty {
	[JsonPropertyName("easy")]
	Easy,

	[JsonPropertyName("medium")]
	Medium,

	[JsonPropertyName("hard")]
	Hard
}
