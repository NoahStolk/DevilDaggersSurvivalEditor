using DevilDaggersSurvivalEditor.Enumerators;
using Newtonsoft.Json;
using System.IO;

namespace DevilDaggersSurvivalEditor.User
{
	[JsonObject(MemberSerialization.OptIn)]
	public class UserSettings
	{
		public const string FileName = "user.json";

		[JsonProperty]
		public string DevilDaggersRootFolder { get; set; } = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers";
		[JsonProperty]
		public bool LockSpawnTile { get; set; }
		[JsonProperty]
		public bool LockGlitchTile { get; set; }
		[JsonProperty]
		public bool EnableEndLoopPreview { get; set; } = true;
		[JsonProperty]
		public bool AskToConfirmArenaGeneration { get; set; } = true;
		[JsonProperty]
		public ReplaceSurvivalAction ReplaceSurvivalAction { get; set; } = ReplaceSurvivalAction.Ask;
		[JsonProperty]
		public bool LoadSurvivalFileOnStartUp { get; set; }

		public string SurvivalFileLocation => Path.Combine(DevilDaggersRootFolder, "mods", "survival");
		public bool SurvivalFileExists => File.Exists(SurvivalFileLocation);
	}
}
