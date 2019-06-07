using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.Utils;
using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class RandomGaps : AbstractRectangularArena
	{
		private float height;
		private int amount = 5;
		private int iterations = 2;

		public float Height
		{
			get => height;
			set => height = MathUtils.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
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

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = X1; i < X2; i++)
				for (int j = Y1; j < Y2; j++)
					tiles[i, j] = Height;

			for (int i = 0; i < Amount; i++)
				tiles[RandomUtils.RandomInt(X1, X2), RandomUtils.RandomInt(Y1, Y2)] = TileUtils.VoidDefault;

			for (int i = 0; i < Iterations; i++)
			{
				for (int j = X1; j < X2; j++)
				{
					for (int k = Y1; k < Y2; k++)
					{
						float tile = tiles[j, k];
						if (tile == TileUtils.VoidDefault)
						{
							if (j > 0 && RandomUtils.Chance(50))
								tiles[j - 1, k] = TileUtils.VoidDefault;
							if (j < Spawnset.ArenaWidth - 1 && RandomUtils.Chance(50))
								tiles[j + 1, k] = TileUtils.VoidDefault;
							if (k > 0 && RandomUtils.Chance(50))
								tiles[j, k - 1] = TileUtils.VoidDefault;
							if (k < Spawnset.ArenaHeight - 1 && RandomUtils.Chance(50))
								tiles[j, k + 1] = TileUtils.VoidDefault;
						}
					}
				}
			}

			return tiles;
		}
	}
}