using DevilDaggersSurvivalEditor.Enumerators;
using Newtonsoft.Json;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.User
{
	[JsonObject(MemberSerialization.OptIn)]
	public class UserSettings
	{
		private const string _fileName = "settings.json";

		public static string FileDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DevilDaggersSurvivalEditor");
		public static string FilePath => Path.Combine(FileDirectory, _fileName);

		[JsonProperty]
		public string DevilDaggersRootFolder { get; set; } = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers";
		[JsonProperty]
		public bool LockSpawnTile { get; set; }
		[JsonProperty]
		public bool EnableEndLoopPreview { get; set; } = true;
		[JsonProperty]
		public bool AskToConfirmArenaGeneration { get; set; } = true;
		[JsonProperty]
		public bool AskToReplaceSurvivalFile { get; set; } = true;
		[JsonProperty]
		public bool AskToDeleteSurvivalFile { get; set; } = true;
		[JsonProperty]
		public ReplaceSurvivalAction ReplaceSurvivalAction { get; set; } = ReplaceSurvivalAction.Ask;
		[JsonProperty]
		public bool LoadSurvivalFileOnStartUp { get; set; }

		public string SurvivalFileLocation => Path.Combine(DevilDaggersRootFolder, "mods", "survival");
		public bool SurvivalFileExists => File.Exists(SurvivalFileLocation);
	}
}
