using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.Utils.Editor;
using System;

namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public class Pyramid : AbstractRectangularArena
	{
		public float StartHeight { get; set; } = -1;
		public float EndHeight { get; set; } = 6;

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();
			SetHeightGlobally(tiles, ArenaUtils.VoidDefault);

			float stepX = (StartHeight - EndHeight) / (X2 - X1 - 1);
			float stepY = (StartHeight - EndHeight) / (Y2 - Y1 - 1);
			for (int i = X1; i < X2; i++)
				for (int j = Y1; j < Y2; j++)
					tiles[i, j] = EndHeight + (Math.Abs(i - Spawnset.ArenaWidth / 2) * stepX + Math.Abs(j - Spawnset.ArenaHeight / 2) * stepY);

			return tiles;
		}
	}
}