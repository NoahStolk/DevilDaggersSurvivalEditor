using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Utils;
using System.Collections.Generic;

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
			set => amount = MathUtils.Clamp(value, 1, 10);
		}
		public int Iterations
		{
			get => iterations;
			set => iterations = MathUtils.Clamp(value, 1, 4);
		}
		public float Steepness
		{
			get => steepness;
			set => steepness = MathUtils.Clamp(value, -5, 5);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			List<ArenaCoord> islandTiles = new List<ArenaCoord>();

			for (int i = 0; i < Amount; i++)
			{
				ArenaCoord coord = new ArenaCoord(RandomUtils.RandomInt(X1, X2), RandomUtils.RandomInt(Y1, Y2));
				islandTiles.Add(coord);
				tiles[coord.X, coord.Y] = RandomUtils.RandomFloat(MinHeight, MaxHeight);
			}

			for (int i = 0; i < Iterations; i++)
			{
				for (int j = X1; j < X2; j++)
				{
					for (int k = Y1; k < Y2; k++)
					{
						if (islandTiles.Contains(new ArenaCoord(j, k)))
						{
							float height = tiles[j, k];

							if (j > 0)
							{
								ArenaCoord coord = new ArenaCoord(j - 1, k);
								if (RandomUtils.Chance(50) && !islandTiles.Contains(coord))
									SetNeighbour(coord, height);
							}
							if (j < Spawnset.ArenaWidth - 1)
							{
								ArenaCoord coord = new ArenaCoord(j + 1, k);
								if (RandomUtils.Chance(50) && !islandTiles.Contains(coord))
									SetNeighbour(coord, height);
							}
							if (k > 0)
							{
								ArenaCoord coord = new ArenaCoord(j, k - 1);
								if (RandomUtils.Chance(50) && !islandTiles.Contains(coord))
									SetNeighbour(coord, height);
							}
							if (k < Spawnset.ArenaHeight - 1)
							{
								ArenaCoord coord = new ArenaCoord(j, k + 1);
								if (RandomUtils.Chance(50) && !islandTiles.Contains(coord))
									SetNeighbour(coord, height);
							}
						}
					}
				}
			}

			return tiles;

			void SetNeighbour(ArenaCoord coord, float parentHeight)
			{
				islandTiles.Add(coord);
				tiles[coord.X, coord.Y] = parentHeight - Steepness;
			}
		}
	}
}