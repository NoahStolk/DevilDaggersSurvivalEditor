using System;

namespace DevilDaggersSurvivalEditor.Code.User
{
	public sealed class UserHandler
	{
		// Must be a field since properties can't be used as out parameters.
		public UserSettings _settings = new UserSettings();

		private static readonly Lazy<UserHandler> _lazy = new Lazy<UserHandler>(() => new UserHandler());

		private UserHandler()
		{
		}

		public static UserHandler Instance => _lazy.Value;
	}
}