namespace GuessWordClient.Models;

public record RoomInfo(
	[property: JsonPropertyName("name")]
	string Name,
	[property: JsonPropertyName("slug")]
	string Slug,
	[property: JsonPropertyName("is_active")]
	bool IsActive,
	[property: JsonPropertyName("finish_at")]
	DateTimeOffset FinishAt,
	[property: JsonPropertyName("stat")]
	RoomInfoStat Stat
);

public record RoomInfoStat(
	[property: JsonPropertyName("word")]
	string Word
);
