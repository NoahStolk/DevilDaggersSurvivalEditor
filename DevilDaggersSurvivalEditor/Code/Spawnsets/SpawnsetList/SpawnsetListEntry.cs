using DevilDaggersCore.Spawnsets.Web;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets.SpawnsetList
{
	internal class SpawnsetListEntry : AbstractListEntry
	{
		internal SpawnsetFile SpawnsetFile { get; set; }
		internal bool HasLeaderboard { get; set; }
	}
}