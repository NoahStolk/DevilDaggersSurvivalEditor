using DevilDaggersSurvivalEditor.Code.Clients;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList
{
	public class SpawnsetListEntry : IListEntry
	{
		public SpawnsetFile SpawnsetFile { get; set; }
		public bool HasCustomLeaderboard { get; set; }
	}
}