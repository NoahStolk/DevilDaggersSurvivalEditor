using System;

namespace DevilDaggersSurvivalEditor.Code.User
{
	public sealed class UserHandler
	{
		// Must be a field since properties can't be used as out parameters.
		public UserSettings settings = new UserSettings();

		private static readonly Lazy<UserHandler> lazy = new Lazy<UserHandler>(() => new UserHandler());

		private UserHandler()
		{
		}

		public static UserHandler Instance => lazy.Value;
	}
}