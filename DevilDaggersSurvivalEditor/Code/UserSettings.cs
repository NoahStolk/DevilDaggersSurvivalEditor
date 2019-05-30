namespace DevilDaggersSurvivalEditor.Code
{
	public class UserSettings
	{
		public const string FileName = "user.json";

		public const string SurvivalFileLocationDefault = @"C:\Program Files (x86)\Steam\steamapps\common\devildaggers\dd";
		public const string CultureDefault = "en-US";

		public string survivalFileLocation = SurvivalFileLocationDefault;
		public string culture = CultureDefault;
	}
}