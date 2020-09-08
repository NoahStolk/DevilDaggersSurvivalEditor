using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets
{
	public class CageRectangular : AbstractRectangularArena
	{
		private float _insideHeight;
		private float _wallHeight = 8;
		private int _wallThickness = 1;

		public float InsideHeight
		{
			get => _insideHeight;
			set => _insideHeight = Math.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public float WallHeight
		{
			get => _wallHeight;
			set => _wallHeight = Math.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public int WallThickness
		{
			get => _wallThickness;
			set => _wallThickness = Math.Clamp(value, 1, 20);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = X1; i < X2; i++)
			{
				for (int j = Y1; j < Y2; j++)
				{
					tiles[i, j] = i >= X1 && i < X1 + WallThickness
							   || i >= X2 - WallThickness && i < X2
							   || j >= Y1 && j < Y1 + WallThickness
							   || j >= Y2 - WallThickness && j < Y2
								? WallHeight : InsideHeight;
				}
			}

			return tiles;
		}
	}
}