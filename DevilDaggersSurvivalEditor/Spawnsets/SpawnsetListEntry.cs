using DevilDaggersSurvivalEditor.Clients;

namespace DevilDaggersSurvivalEditor.Spawnsets
{
	public class SpawnsetListEntry : IListEntry
	{
		public SpawnsetListEntry(SpawnsetFile spawnsetFile, bool hasCustomLeaderboard)
		{
			SpawnsetFile = spawnsetFile;
			HasCustomLeaderboard = hasCustomLeaderboard;
		}

		public SpawnsetFile SpawnsetFile { get; set; }
		public bool HasCustomLeaderboard { get; set; }
	}
}