using DevilDaggersCore.Utils;
using DevilDaggersSurvivalEditor.Utils;
using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets
{
	public class RandomNoise : AbstractRectangularArena
	{
		private float _minHeight;
		private float _maxHeight = 16;

		public float MinHeight
		{
			get => _minHeight;
			set => _minHeight = Math.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public float MaxHeight
		{
			get => _maxHeight;
			set => _maxHeight = Math.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = X1; i < X2; i++)
			{
				for (int j = Y1; j < Y2; j++)
					tiles[i, j] = RandomUtils.RandomFloat(MinHeight, MaxHeight);
			}

			return tiles;
		}
	}
}
