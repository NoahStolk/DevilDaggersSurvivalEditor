using DevilDaggersCore.Spawnsets;
using Newtonsoft.Json;
using System.IO;

namespace DevilDaggersSurvivalEditor.User
{
	[JsonObject(MemberSerialization.OptIn)]
	public class UserSettings
	{
		public const string FileName = "user.json";

		[JsonProperty]
		public string SurvivalFileRootFolder { get; set; } = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\dd";
		[JsonProperty]
		public bool LockSpawnTile { get; set; }
		[JsonProperty]
		public bool LockGlitchTile { get; set; }
		[JsonProperty]
		public bool EnableEndLoopPreview { get; set; } = true;
		[JsonProperty]
		public bool AskToConfirmArenaGeneration { get; set; } = true;

		public string SurvivalFileLocation => Path.Combine(SurvivalFileRootFolder, "survival");
		public bool SurvivalFileExists => File.Exists(SurvivalFileLocation);
		public bool SurvivalFileIsValid => SurvivalFileExists && Spawnset.TryParse(File.ReadAllBytes(UserHandler.Instance.Settings.SurvivalFileLocation), out _);
	}
}