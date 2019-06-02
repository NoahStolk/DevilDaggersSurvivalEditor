using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.Utils.Editor;
using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public class Islands : AbstractRectangularArena
	{
		public float MinHeight { get; set; }
		public float MaxHeight { get; set; } = 5;
		public int Amount { get; set; } = 5;
		public int Iterations { get; set; } = 2;
		public float Steepness { get; set; } = 0.1f;

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();
			SetHeightGlobally(tiles, ArenaUtils.VoidDefault);

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

			// Make sure the player doesn't spawn in the void
			if (tiles[Spawnset.ArenaWidth / 2, Spawnset.ArenaHeight / 2] < -1)
				tiles[Spawnset.ArenaWidth / 2, Spawnset.ArenaHeight / 2] = 0;

			return tiles;
		}
	}
}