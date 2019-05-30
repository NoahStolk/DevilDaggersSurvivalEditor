using DevilDaggersSurvivalEditor.Utils;

namespace DevilDaggersSurvivalEditor.Models
{
	public class UserSettings
	{
		public string survivalFileLocation = UserSettingsUtils.SurvivalFileLocationDefault;
		public string culture = UserSettingsUtils.DefaultCulture;
	}
}