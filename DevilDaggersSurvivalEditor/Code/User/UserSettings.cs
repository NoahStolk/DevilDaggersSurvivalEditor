namespace DevilDaggersSurvivalEditor.Code.User
{
	public class UserSettings
	{
		public const string FileName = "user.json";

		public const string SurvivalFileLocationDefault = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\dd";
		public string SurvivalFileLocation { get; set; } = SurvivalFileLocationDefault;

		public bool LockSpawnTile { get; set; }
		public bool LockGlitchTile { get; set; }
	}
}