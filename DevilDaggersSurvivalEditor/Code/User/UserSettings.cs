using DevilDaggersCore.Spawnsets;
using NetBase.Utils;
using Newtonsoft.Json;
using System.IO;

namespace DevilDaggersSurvivalEditor.Code.User
{
	[JsonObject(MemberSerialization.OptIn)]
	public class UserSettings
	{
		public const string FileName = "user.json";
		public const string SurvivalFileRootFolderDefault = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\dd";

		[JsonProperty]
		public string SurvivalFileRootFolder { get; set; } = SurvivalFileRootFolderDefault;
		[JsonProperty]
		public bool LockSpawnTile { get; set; }
		[JsonProperty]
		public bool LockGlitchTile { get; set; }

		private int endWavePreviewAmount = 3;
		[JsonProperty]
		public int EndWavePreviewAmount
		{
			get => endWavePreviewAmount;
			set
			{
				endWavePreviewAmount = MathUtils.Clamp(value, 1, 100);
			}
		}

		public string SurvivalFileLocation => Path.Combine(SurvivalFileRootFolder, "survival");
		public bool SurvivalFileExists => File.Exists(SurvivalFileLocation);
		public bool SurvivalFileIsValid => SurvivalFileExists && Spawnset.TryParse(new FileStream(UserHandler.Instance.settings.SurvivalFileLocation, FileMode.Open, FileAccess.Read), out _);
	}
}