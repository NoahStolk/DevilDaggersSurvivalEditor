using System;

namespace DevilDaggersSurvivalEditor.Code.User
{
	public sealed class UserSettingsHandler
	{
		// Must be a field since properties can't be used as out parameters.
		public UserSettings userSettings = new UserSettings();

		private static readonly Lazy<UserSettingsHandler> lazy = new Lazy<UserSettingsHandler>(() => new UserSettingsHandler());
		public static UserSettingsHandler Instance => lazy.Value;

		private UserSettingsHandler()
		{
		}
	}
}