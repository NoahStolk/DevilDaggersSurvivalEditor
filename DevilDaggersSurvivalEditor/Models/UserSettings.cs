using DevilDaggersSurvivalEditor.Utils;

namespace DevilDaggersSurvivalEditor.Models
{
	public class UserSettings
	{
		public string SurvivalFileLocation = UserSettingsUtils.SurvivalFileLocationDefault;
		public string culture = UserSettingsUtils.DefaultCulture;
	}
}