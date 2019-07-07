using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.Arena;
using System;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets
{
	public sealed class SpawnsetHandler
	{
		// Must be a field since properties can't be used as out parameters.
		public Spawnset spawnset;

		private static readonly Lazy<SpawnsetHandler> lazy = new Lazy<SpawnsetHandler>(() => new SpawnsetHandler());
		public static SpawnsetHandler Instance => lazy.Value;

		private SpawnsetHandler() => spawnset = new Spawnset { ArenaTiles = ArenaPresetHandler.Instance.DefaultPreset.GetTiles() };
	}
}