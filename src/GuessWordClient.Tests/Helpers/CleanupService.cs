namespace GuessWordClient.Tests.Helpers;

public class CleanupService(IGuessWordClient client) : IHostedService {
	public async Task StartAsync(CancellationToken cancellationToken) {
		await Cleanup(cancellationToken);
	}

	public Task StopAsync(CancellationToken cancellationToken) {
		return Task.CompletedTask;
	}

	private async Task Cleanup(CancellationToken cancellationToken) {
		IReadOnlyCollection<RoomInfo> rooms = await client.GetRooms(cancellationToken);
		foreach ((string _, string slug, bool _, DateTimeOffset _, RoomInfoStat _) in rooms) {
			await client.DeleteRoom(slug, cancellationToken);
		}
	}
}
