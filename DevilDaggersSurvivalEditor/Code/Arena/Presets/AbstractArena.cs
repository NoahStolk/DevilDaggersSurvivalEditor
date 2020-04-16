﻿using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Code.Spawnsets;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	internal abstract class AbstractArena
	{
		internal abstract bool IsFull { get; }

		internal abstract float[,] GetTiles();

		private protected float[,] CreateArenaArray()
		{
			// Startup of the application, return empty array.
			if (App.Instance == null || App.Instance.MainWindow == null)
				return new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];

			// Clear previous is off, return the old arena.
			if (!App.Instance.MainWindow.SpawnsetArena.ClearPreviousCheckBox.IsChecked == true)
				return SpawnsetHandler.Instance.spawnset.ArenaTiles;

			// Return void arena.
			float[,] voidArena = new float[Spawnset.ArenaWidth, Spawnset.ArenaHeight];
			SetHeightGlobally(voidArena, TileUtils.VoidDefault);
			return voidArena;
		}

		private protected void SetHeightGlobally(float[,] arenaArray, float height)
		{
			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					arenaArray[i, j] = height;
		}
	}
}