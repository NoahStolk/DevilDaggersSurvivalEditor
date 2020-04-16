﻿using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	internal class CageRectangular : AbstractOrientedRectangularArena
	{
		private float insideHeight;
		private float wallHeight = 8;
		private int wallThickness = 1;

		internal float InsideHeight
		{
			get => insideHeight;
			set => insideHeight = MathUtils.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}
		internal float WallHeight
		{
			get => wallHeight;
			set => wallHeight = MathUtils.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}
		internal int WallThickness
		{
			get => wallThickness;
			set => wallThickness = MathUtils.Clamp(value, 1, 20);
		}

		internal override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = X1; i < X2; i++)
				for (int j = Y1; j < Y2; j++)
					tiles[i, j] = i >= X1 && i < X1 + WallThickness
							   || i >= X2 - WallThickness && i < X2
							   || j >= Y1 && j < Y1 + WallThickness
							   || j >= Y2 - WallThickness && j < Y2
								? WallHeight : InsideHeight;

			return tiles;
		}
	}
}