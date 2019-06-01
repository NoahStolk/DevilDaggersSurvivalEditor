using DevilDaggersCore.Spawnset;
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
			float[,] tiles = VoidArena();

			for (int i = 0; i < Amount; i++)
				tiles[RandomUtils.RandomInt(X1, X2), RandomUtils.RandomInt(Y1, Y2)] = RandomUtils.RandomFloat(MinHeight, MaxHeight);

			for (int i = 0; i < Iterations; i++)
			{
				for (int j = X1; j < X2; j++)
				{
					for (int k = Y1; k < Y2; k++)
					{
						float tile = tiles[j, k];
						if (tile >= -1) // Not void
						{
							if (RandomUtils.Chance(50) && j > 0)
								tiles[j - 1, k] = tile - Steepness;
							if (RandomUtils.Chance(50) && j < Spawnset.ArenaWidth - 1)
								tiles[j + 1, k] = tile - Steepness;
							if (RandomUtils.Chance(50) && k > 0)
								tiles[j, k - 1] = tile - Steepness;
							if (RandomUtils.Chance(50) && k < Spawnset.ArenaHeight - 1)
								tiles[j, k + 1] = tile - Steepness;
						}
					}
				}
			}

			if (tiles[Spawnset.ArenaWidth / 2, Spawnset.ArenaHeight / 2] < -1)
				tiles[Spawnset.ArenaWidth / 2, Spawnset.ArenaHeight / 2] = 0;

			return tiles;
		}
	}
}