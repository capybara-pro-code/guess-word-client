namespace GuessWordClient.Tests;

[Trait("Category", "Huge")]
public class GuessWordClientTests(IGuessWordClient client, IOptions<GuessWordClientOptions> options) {
	[Theory]
	[InlineData("Test-CreateRoomReturnsRoomId-Easy", RoomDifficulty.Easy)]
	[InlineData("Test-CreateRoomReturnsRoomId-Medium", RoomDifficulty.Medium)]
	[InlineData("Test-CreateRoomReturnsRoomId-Hard", RoomDifficulty.Hard)]
	public async Task CreateRoomReturnsRoomId(string name, RoomDifficulty difficulty) {
		RoomInfo room = await client.CreateRoom(name, difficulty, CancellationToken.None);

		room.Should().Match<RoomInfo>(r =>
			                              r.Name.Equals(name)
			                              && r.IsActive
			                              && r.FinishAt - DateTimeOffset.UtcNow.AddDays(1) < TimeSpan.FromMinutes(1)
		);
	}

	[Fact]
	public async Task DeleteRoomCheck() {
		RoomInfo room = await client.CreateRoom("Test-DeleteRoomCheck", RoomDifficulty.Easy, CancellationToken.None);

		await client.DeleteRoom(room.Slug, CancellationToken.None);

		IReadOnlyCollection<RoomInfo> rooms = await client.GetRooms(CancellationToken.None);
		rooms.Should().NotContain(r => r.Slug.Equals(room.Slug));
	}

	[Fact]
	public async Task GetMeReturnsNameFromOptions() {
		MeInfo me = await client.GetMe(CancellationToken.None);

		me.Username.Should().Be(options.Value.Username);
	}

	[Fact]
	public async Task GuessNonExistentWordReturnsError() {
		RoomInfo room = await client.CreateRoom("Test-GuessWrongWordReturnsError", RoomDifficulty.Easy, CancellationToken.None);
		room = await client.GetRoom(room.Slug, true, CancellationToken.None);

		var act = new Func<Task>(() => client.Guess(room.Slug, "словокоторогонет", CancellationToken.None));

		ExceptionAssertions<ErrorResultException>? exception = await act.Should().ThrowAsync<ErrorResultException>();
		exception.Which.ErrorCode.Should().Be(ErrorCode.WordNotFound);
	}

	[Fact]
	public async Task GuessRightWordReturnsOrder1() {
		RoomInfo room = await client.CreateRoom("Test-GuessRightWordReturnsOrder1", RoomDifficulty.Easy, CancellationToken.None);
		room = await client.GetRoom(room.Slug, true, CancellationToken.None);

		GuessResult result1 = await client.Guess(room.Slug, room.Stat.Word, CancellationToken.None);
		GuessResult result2 = await client.Guess(room.Slug, room.Stat.Word, CancellationToken.None);

		result1.Should().Match<GuessResult>(
			r => r.Order == 1 && !r.AlreadyGuessed,
			"First try should not be AlreadyGuessed"
		);
		result2.Should().Match<GuessResult>(
			r => r.Order == 1 && r.AlreadyGuessed,
			"Second try should be AlreadyGuessed"
		);
	}
}
