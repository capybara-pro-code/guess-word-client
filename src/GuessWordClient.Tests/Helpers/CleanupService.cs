namespace GuessWordClient.Tests.Helpers;

public class CleanupService(IGuessWordClient client, TestsValues testsValues) : IHostedService {
	public async Task StartAsync(CancellationToken cancellationToken) {
		await Cleanup(cancellationToken);
	}

	public async Task StopAsync(CancellationToken cancellationToken) {
		await Cleanup(cancellationToken);
	}

	private async Task Cleanup(CancellationToken cancellationToken) {
		IReadOnlyCollection<RoomInfo> rooms = await client.GetRooms(cancellationToken);
		foreach (RoomInfo room in rooms
			         // comment this line to delete all test rooms
			         .Where(r => r.Name.StartsWith(testsValues.RoomsPrefix))
		        ) {
			await client.DeleteRoom(room.Slug, cancellationToken);
		}
	}
}
