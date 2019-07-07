using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.Spawnsets;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public abstract class AbstractArena
	{
		public abstract bool IsFull { get; }

		public abstract float[,] GetTiles();

		protected float[,] CreateArenaArray()
		{
			// Startup of the application, return empty array
			if (Program.App == null || Program.App.MainWindow == null)
				return new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

			// Clear previous is off, return the old arena
			if (!Program.App.MainWindow.SpawnsetArena.ClearPreviousCheckBox.IsChecked == true)
				return SpawnsetHandler.Instance.spawnset.ArenaTiles;

			// Return void arena
			float[,] voidArena = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];
			SetHeightGlobally(voidArena, TileUtils.VoidDefault);
			return voidArena;
		}

		protected void SetHeightGlobally(float[,] arenaArray, float height)
		{
			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					arenaArray[i, j] = height;
		}
	}
}