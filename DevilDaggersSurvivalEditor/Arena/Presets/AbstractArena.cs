using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.Utils;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public abstract class AbstractArena
{
	protected AbstractArena(int dimension)
	{
		Dimension = dimension;
	}

	public int Dimension { get; }

	public abstract bool IsFull { get; }

	public abstract float[,] GetTiles();

	protected float[,] CreateArenaArray()
	{
		// Startup of the application, return empty array.
		if (App.Instance == null || App.Instance.MainWindow == null)
			return new float[Dimension, Dimension];

		// Clear previous is off, return the old arena.
		if (App.Instance.MainWindow.SpawnsetArena.ClearPreviousCheckBox.IsChecked == false)
			return SpawnsetHandler.Instance.Spawnset.ArenaTiles;

		// Return void arena.
		float[,] voidArena = new float[Dimension, Dimension];
		SetHeightGlobally(voidArena, TileUtils.VoidDefault);
		return voidArena;
	}

	protected void SetHeightGlobally(float[,] arenaArray, float height)
	{
		for (int i = 0; i < Dimension; i++)
		{
			for (int j = 0; j < Dimension; j++)
				arenaArray[i, j] = height;
		}
	}
}
