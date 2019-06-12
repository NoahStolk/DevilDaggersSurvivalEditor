using DevilDaggersCore.Spawnset;
using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class RandomIslands : AbstractRectangularArena
	{
		private float minHeight;
		private float maxHeight = 5;
		private int amount = 5;
		private int iterations = 2;
		private float steepness = 0.1f;

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
		public int Amount
		{
			get => amount;
			set => amount = MathUtils.Clamp(value, 1, 50);
		}
		public int Iterations
		{
			get => iterations;
			set => iterations = MathUtils.Clamp(value, 1, 20);
		}
		public float Steepness
		{
			get => steepness;
			set => steepness = MathUtils.Clamp(value, -5, 5);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = 0; i < Amount; i++)
				tiles[RandomUtils.RandomInt(X1, X2), RandomUtils.RandomInt(Y1, Y2)] = RandomUtils.RandomFloat(MinHeight, MaxHeight);

			for (int i = 0; i < Iterations; i++)
			{
				for (int j = X1; j < X2; j++)
				{
					for (int k = Y1; k < Y2; k++)
					{
						float tile = tiles[j, k];
						if (tile >= -1)
						{
							if (j > 0 && RandomUtils.Chance(50))
								tiles[j - 1, k] = tile - Steepness;
							if (j < Spawnset.ArenaWidth - 1 && RandomUtils.Chance(50))
								tiles[j + 1, k] = tile - Steepness;
							if (k > 0 && RandomUtils.Chance(50))
								tiles[j, k - 1] = tile - Steepness;
							if (k < Spawnset.ArenaHeight - 1 && RandomUtils.Chance(50))
								tiles[j, k + 1] = tile - Steepness;
						}
					}
				}
			}

			return tiles;
		}
	}
}