using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.Arena;
using System;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets
{
	public sealed class SpawnsetHandler
	{
		private bool unsavedChanges = false;
		public bool UnsavedChanges
		{
			get => unsavedChanges;
			set
			{
				unsavedChanges = value;
				Program.App.UpdateMainWindowTitle();
			}
		}

		public string SpawnsetName { get; private set; } = "(new spawnset)";
		public string SpawnsetFileLocation { get; private set; } = string.Empty;

		// Must be a field since properties can't be used as out parameters.
		public Spawnset spawnset;

		private static readonly Lazy<SpawnsetHandler> lazy = new Lazy<SpawnsetHandler>(() => new SpawnsetHandler());
		public static SpawnsetHandler Instance => lazy.Value;

		private SpawnsetHandler()
		{
			spawnset = new Spawnset { ArenaTiles = ArenaPresetHandler.Instance.DefaultPreset.GetTiles() };
		}

		public void UpdateSpawnsetState(string name, string fileLocation)
		{
			UnsavedChanges = false;

			SpawnsetName = name;
			SpawnsetFileLocation = fileLocation;

			Program.App.UpdateMainWindowTitle();
		}
	}
}