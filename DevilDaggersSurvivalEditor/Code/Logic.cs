using DevilDaggersCore.Spawnset;
using System;

namespace DevilDaggersSurvivalEditor.Code
{
	public sealed class Logic
	{
		public UserSettings userSettings = new UserSettings();
		public Spawnset spawnset = new Spawnset();

		private static readonly Lazy<Logic> lazy = new Lazy<Logic>(() => new Logic());
		public static Logic Instance => lazy.Value;

		private Logic()
		{
		}
	}
}