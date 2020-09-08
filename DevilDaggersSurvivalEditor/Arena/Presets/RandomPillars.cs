using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Utils;
using DevilDaggersSurvivalEditor.Utils;
using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets
{
	public class RandomPillars : AbstractRectangularArena
	{
		private float _minHeight = 5;
		private float _maxHeight = 7;
		private int _minThickness = 1;
		private int _maxThickness = 2;
		private int _count = 5;

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

		public int MinThickness
		{
			get => _minThickness;
			set => _minThickness = Math.Clamp(value, 1, 15);
		}

		public int MaxThickness
		{
			get => _maxThickness;
			set => _maxThickness = Math.Clamp(value, 1, 15);
		}

		public int Count
		{
			get => _count;
			set => _count = Math.Clamp(value, 1, 200);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = 0; i < _count; i++)
			{
				float height = RandomUtils.RandomFloat(_minHeight, _maxHeight);
				int x = RandomUtils.RandomInt(X1, X2);
				int y = RandomUtils.RandomInt(Y1, Y2);

				int thickness = RandomUtils.RandomInt(_minThickness, _maxThickness + 1);

				int min = -(int)Math.Floor((double)thickness / 2);
				int max = (int)Math.Ceiling((double)thickness / 2);
				for (int j = min; j < max; j++)
				{
					for (int k = min; k < max; k++)
						tiles[Math.Clamp(x + j, 0, Spawnset.ArenaWidth - 1), Math.Clamp(y + k, 0, Spawnset.ArenaHeight - 1)] = height;
				}
			}

			return tiles;
		}
	}
}