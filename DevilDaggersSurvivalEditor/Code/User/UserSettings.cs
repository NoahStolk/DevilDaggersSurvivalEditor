using DevilDaggersCore.Spawnsets;
using Newtonsoft.Json;
using System.IO;

namespace DevilDaggersSurvivalEditor.Code.User
{
	[JsonObject(MemberSerialization.OptIn)]
	internal class UserSettings
	{
		internal const string FileName = "user.json";

		[JsonProperty]
		internal string SurvivalFileRootFolder { get; set; } = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\dd";
		[JsonProperty]
		internal bool LockSpawnTile { get; set; }
		[JsonProperty]
		internal bool LockGlitchTile { get; set; }
		[JsonProperty]
		internal bool EnableEndLoopPreview { get; set; } = true;

		internal string SurvivalFileLocation => Path.Combine(SurvivalFileRootFolder, "survival");
		internal bool SurvivalFileExists => File.Exists(SurvivalFileLocation);
		internal bool SurvivalFileIsValid => SurvivalFileExists && Spawnset.TryParse(new FileStream(UserHandler.Instance.settings.SurvivalFileLocation, FileMode.Open, FileAccess.Read), out _);
	}
}