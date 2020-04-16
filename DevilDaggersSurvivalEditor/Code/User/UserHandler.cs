using System;

namespace DevilDaggersSurvivalEditor.Code.User
{
	internal sealed class UserHandler
	{
		// Must be a field since properties can't be used as out parameters.
		internal UserSettings settings = new UserSettings();

		private static readonly Lazy<UserHandler> lazy = new Lazy<UserHandler>(() => new UserHandler());
		internal static UserHandler Instance => lazy.Value;

		private UserHandler()
		{
		}
	}
}