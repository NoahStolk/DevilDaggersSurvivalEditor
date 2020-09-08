using DevilDaggersSurvivalEditor.Clients;

namespace DevilDaggersSurvivalEditor.Spawnsets.SpawnsetList
{
	public class SpawnsetListEntry : IListEntry
	{
		public SpawnsetFile SpawnsetFile { get; set; }
		public bool HasCustomLeaderboard { get; set; }
	}
}