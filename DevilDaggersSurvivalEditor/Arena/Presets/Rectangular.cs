using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets
{
	public class Rectangular : AbstractRectangularArena
	{
		private float _height;

		public float Height
		{
			get => _height;
			set => _height = Math.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = X1; i < X2; i++)
			{
				for (int j = Y1; j < Y2; j++)
					tiles[i, j] = Height;
			}

			return tiles;
		}
	}
}