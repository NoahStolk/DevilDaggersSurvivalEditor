using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.Utils.Editor;
using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public class Gaps : AbstractRectangularArena
	{
		public int Amount { get; set; } = 5;
		public int Iterations { get; set; } = 2;

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = 0; i < Amount; i++)
				tiles[RandomUtils.RandomInt(X1, X2), RandomUtils.RandomInt(Y1, Y2)] = ArenaUtils.VoidDefault;

			for (int i = 0; i < Iterations; i++)
			{
				for (int j = X1; j < X2; j++)
				{
					for (int k = Y1; k < Y2; k++)
					{
						float tile = tiles[j, k];
						if (tile == ArenaUtils.VoidDefault)
						{
							if (RandomUtils.Chance(50) && j > 0)
								tiles[j - 1, k] = ArenaUtils.VoidDefault;
							if (RandomUtils.Chance(50) && j < Spawnset.ArenaWidth - 1)
								tiles[j + 1, k] = ArenaUtils.VoidDefault;
							if (RandomUtils.Chance(50) && k > 0)
								tiles[j, k - 1] = ArenaUtils.VoidDefault;
							if (RandomUtils.Chance(50) && k < Spawnset.ArenaHeight - 1)
								tiles[j, k + 1] = ArenaUtils.VoidDefault;
						}
					}
				}
			}

			// Make sure the player doesn't spawn in the void
			if (tiles[Spawnset.ArenaWidth / 2, Spawnset.ArenaHeight / 2] == ArenaUtils.VoidDefault)
				tiles[Spawnset.ArenaWidth / 2, Spawnset.ArenaHeight / 2] = 0;

			return tiles;
		}
	}
}