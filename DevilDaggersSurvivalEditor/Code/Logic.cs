using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.GUI.UserControls;
using System;

namespace DevilDaggersSurvivalEditor.Code
{
	public sealed class Logic
	{
		public SpawnsetArena UserControlArena { get; set; }
		public SpawnsetSettings UserControlSettings { get; set; }
		public SpawnsetSpawns UserControlSpawns { get; set; }

		public UserSettings userSettings = new UserSettings();
		public Spawnset spawnset = new Spawnset();

		private static readonly Lazy<Logic> lazy = new Lazy<Logic>(() => new Logic());
		public static Logic Instance => lazy.Value;

		private Logic()
		{
		}
	}
}