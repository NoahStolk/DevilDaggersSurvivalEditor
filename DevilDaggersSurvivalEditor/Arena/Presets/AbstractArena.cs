using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.Utils;

namespace DevilDaggersSurvivalEditor.Arena.Presets
{
	public abstract class AbstractArena
	{
		public abstract bool IsFull { get; }

		public abstract float[,] GetTiles();

		protected static float[,] CreateArenaArray()
		{
			// Startup of the application, return empty array.
			if (App.Instance == null || App.Instance.MainWindow == null)
				return new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

			// Clear previous is off, return the old arena.
			if (App.Instance.MainWindow.SpawnsetArena.ClearPreviousCheckBox.IsChecked == false)
				return SpawnsetHandler.Instance.Spawnset.ArenaTiles;

			// Return void arena.
			float[,] voidArena = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];
			SetHeightGlobally(voidArena, TileUtils.VoidDefault);
			return voidArena;
		}

		protected static void SetHeightGlobally(float[,] arenaArray, float height)
		{
			for (int i = 0; i < Spawnset.ArenaWidth; i++)
			{
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					arenaArray[i, j] = height;
			}
		}
	}
}
