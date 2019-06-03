using DevilDaggersCore.Spawnset;
using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class Gaps : AbstractRectangularArena
	{
		private float height;
		private int amount = 5;
		private int iterations = 2;

		public float Height
		{
			get => height;
			set => height = MathUtils.Clamp(value, ArenaCoord.TileMin, ArenaCoord.TileMax);
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
			SetHeightGlobally(tiles, Height);

			for (int i = 0; i < Amount; i++)
				tiles[RandomUtils.RandomInt(X1, X2), RandomUtils.RandomInt(Y1, Y2)] = ArenaCoord.VoidDefault;

			for (int i = 0; i < Iterations; i++)
			{
				for (int j = X1; j < X2; j++)
				{
					for (int k = Y1; k < Y2; k++)
					{
						float tile = tiles[j, k];
						if (tile == ArenaCoord.VoidDefault)
						{
							if (j > 0 && RandomUtils.Chance(50))
								tiles[j - 1, k] = ArenaCoord.VoidDefault;
							if (j < Spawnset.ArenaWidth - 1 && RandomUtils.Chance(50))
								tiles[j + 1, k] = ArenaCoord.VoidDefault;
							if (k > 0 && RandomUtils.Chance(50))
								tiles[j, k - 1] = ArenaCoord.VoidDefault;
							if (k < Spawnset.ArenaHeight - 1 && RandomUtils.Chance(50))
								tiles[j, k + 1] = ArenaCoord.VoidDefault;
						}
					}
				}
			}

			// Make sure the player doesn't spawn in the void
			if (tiles[Spawnset.ArenaWidth / 2, Spawnset.ArenaHeight / 2] == ArenaCoord.VoidDefault)
				tiles[Spawnset.ArenaWidth / 2, Spawnset.ArenaHeight / 2] = 0;

			return tiles;
		}
	}
}