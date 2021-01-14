using System;

namespace DevilDaggersSurvivalEditor.User
{
	public sealed class UserHandler
	{
		private static readonly Lazy<UserHandler> _lazy = new Lazy<UserHandler>(() => new UserHandler());

		private UserHandler()
		{
		}

		public static UserHandler Instance => _lazy.Value;

		public UserSettings Settings { get; set; } = new UserSettings();
	}
}
