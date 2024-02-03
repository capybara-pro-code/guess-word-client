namespace GuessWordClient;

public interface IGuessWordClient {
	Task<MeInfo> GetMe(CancellationToken cancellationToken);
	Task<RoomInfo> CreateRoom(string name, RoomDifficulty difficulty, CancellationToken cancellationToken);
	Task<IReadOnlyCollection<RoomInfo>> GetRooms(CancellationToken cancellationToken);
	Task DeleteRoom(string slug, CancellationToken cancellationToken);
	Task<GuessResult> Guess(string roomSlug, string word, CancellationToken cancellationToken);
	Task<RoomInfo> GetRoom(string slug, bool includeWord, CancellationToken cancellationToken);
}
