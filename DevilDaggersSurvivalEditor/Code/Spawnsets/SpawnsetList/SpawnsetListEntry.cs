using DevilDaggersCore.Spawnsets.Web;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList
{
	public class SpawnsetListEntry : AbstractListEntry
	{
		public SpawnsetFile SpawnsetFile { get; set; }
		public bool HasLeaderboard { get; set; }
	}
}