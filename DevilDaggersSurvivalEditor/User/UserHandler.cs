using System;

namespace DevilDaggersSurvivalEditor.User
{
	public sealed class UserHandler
	{
		private static readonly Lazy<UserHandler> _lazy = new(() => new());

		private UserHandler()
		{
		}

		public static UserHandler Instance => _lazy.Value;

		public UserSettings Settings { get; set; } = new();
	}
}
