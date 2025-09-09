namespace GuessWordClient.Tests;

[Trait("Category", "Huge")]
public class GuessWordClientTests(IGuessWordClient client, IOptions<GuessWordClientOptions> options, TestsValues testsValues) {
	[Theory]
	[InlineData("CreateRoomReturnsRoomId-Easy", RoomDifficulty.Easy)]
	[InlineData("CreateRoomReturnsRoomId-Medium", RoomDifficulty.Medium)]
	[InlineData("CreateRoomReturnsRoomId-Hard", RoomDifficulty.Hard)]
	public async Task CreateRoomReturnsRoomId(string name, RoomDifficulty difficulty) {
		RoomInfo room = await client.CreateRoom($"{testsValues.RoomsPrefix}_{name}", difficulty, CancellationToken.None);

		room = await client.GetRoom(room.Slug, false, CancellationToken.None);

		room.ShouldSatisfyAllConditions(
			() => room.Name.ShouldBe($"{testsValues.RoomsPrefix}_{name}"),
			() => room.IsActive.ShouldBeTrue(),
			() => (room.FinishAt - DateTimeOffset.UtcNow.AddDays(1)).ShouldBeLessThan(TimeSpan.FromMinutes(1))
		);
	}

	[Fact]
	public async Task DeleteRoomCheck() {
		RoomInfo room = await client.CreateRoom($"{testsValues.RoomsPrefix}_DeleteRoomCheck", RoomDifficulty.Easy, CancellationToken.None);

		await client.DeleteRoom(room.Slug, CancellationToken.None);
		IReadOnlyCollection<RoomInfo> rooms = await client.GetRooms(CancellationToken.None);

		rooms.ShouldNotContain(r => r.Slug.Equals(room.Slug));
	}

	[Fact]
	public async Task GetMeReturnsNameFromOptions() {
		MeInfo me = await client.GetMe(CancellationToken.None);

		me.Username.ShouldBe(options.Value.Username);
	}

	[Fact]
	public async Task GuessNonExistentWordReturnsError() {
		RoomInfo room = await client.CreateRoom($"{testsValues.RoomsPrefix}_GuessWrongWordReturnsError", RoomDifficulty.Easy, CancellationToken.None);
		room = await client.GetRoom(room.Slug, true, CancellationToken.None);

		var act = new Func<Task>(() => client.Guess(room.Slug, "словокоторогонет", CancellationToken.None));

		var exception = await Should.ThrowAsync<ErrorResultException>(act);
		exception.ErrorCode.ShouldBe(ErrorCode.WordNotFound);
	}

	[Fact]
	public async Task GuessRightWordReturnsOrder1() {
		RoomInfo room = await client.CreateRoom($"{testsValues.RoomsPrefix}_GuessRightWordReturnsOrder1", RoomDifficulty.Easy, CancellationToken.None);
		room = await client.GetRoom(room.Slug, true, CancellationToken.None);

		GuessResult firstTry = await client.Guess(room.Slug, room.Stat.Word, CancellationToken.None);
		GuessResult secondTry = await client.Guess(room.Slug, room.Stat.Word, CancellationToken.None);

		firstTry.ShouldSatisfyAllConditions(
			() => firstTry.Order.ShouldBe((uint)1),
			() => firstTry.AlreadyGuessed.ShouldBeFalse()
		);
		secondTry.ShouldSatisfyAllConditions(
			() => secondTry.Order.ShouldBe((uint)1),
			() => secondTry.AlreadyGuessed.ShouldBeTrue()
		);
	}
}
