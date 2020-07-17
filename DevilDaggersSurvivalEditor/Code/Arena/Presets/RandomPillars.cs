using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Utils;
using System;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class RandomPillars : AbstractRectangularArena
	{
		private float minHeight = 5;
		private float maxHeight = 7;
		private int minThickness = 1;
		private int maxThickness = 2;
		private int count = 5;

		public float MinHeight
		{
			get => minHeight;
			set => minHeight = MathUtils.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}
		public float MaxHeight
		{
			get => maxHeight;
			set => maxHeight = MathUtils.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}
		public int MinThickness
		{
			get => minThickness;
			set => minThickness = MathUtils.Clamp(value, 1, 15);
		}
		public int MaxThickness
		{
			get => maxThickness;
			set => maxThickness = MathUtils.Clamp(value, 1, 15);
		}
		public int Count
		{
			get => count;
			set => count = MathUtils.Clamp(value, 1, 200);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = 0; i < count; i++)
			{
				float height = RandomUtils.RandomFloat(minHeight, maxHeight);
				int x = RandomUtils.RandomInt(X1, X2);
				int y = RandomUtils.RandomInt(Y1, Y2);

				int thickness = RandomUtils.RandomInt(minThickness, maxThickness + 1);

				int min = -(int)Math.Floor((double)thickness / 2);
				int max = (int)Math.Ceiling((double)thickness / 2);
				for (int j = min; j < max; j++)
					for (int k = min; k < max; k++)
						tiles[MathUtils.Clamp(x + j, 0, Spawnset.ArenaWidth - 1), MathUtils.Clamp(y + k, 0, Spawnset.ArenaHeight - 1)] = height;
			}

			return tiles;
		}
	}
}