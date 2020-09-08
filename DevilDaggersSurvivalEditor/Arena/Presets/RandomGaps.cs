using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Utils;
using DevilDaggersSurvivalEditor.Utils;
using System;
using System.Collections.Generic;

namespace DevilDaggersSurvivalEditor.Arena.Presets
{
	public class RandomGaps : AbstractRectangularArena
	{
		private float _height;
		private int _amount = 5;
		private int _iterations = 2;

		public float Height
		{
			get => _height;
			set => _height = Math.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public int Amount
		{
			get => _amount;
			set => _amount = Math.Clamp(value, 1, 10);
		}

		public int Iterations
		{
			get => _iterations;
			set => _iterations = Math.Clamp(value, 1, 4);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = X1; i < X2; i++)
			{
				for (int j = Y1; j < Y2; j++)
					tiles[i, j] = Height;
			}

			List<ArenaCoord> gapTiles = new List<ArenaCoord>();

			for (int i = 0; i < Amount; i++)
			{
				ArenaCoord coord = new ArenaCoord(RandomUtils.RandomInt(X1, X2), RandomUtils.RandomInt(Y1, Y2));
				gapTiles.Add(coord);
				tiles[coord.X, coord.Y] = TileUtils.VoidDefault;
			}

			for (int i = 0; i < Iterations; i++)
			{
				for (int j = X1; j < X2; j++)
				{
					for (int k = Y1; k < Y2; k++)
					{
						if (gapTiles.Contains(new ArenaCoord(j, k)))
						{
							if (j > 0)
							{
								ArenaCoord coord = new ArenaCoord(j - 1, k);
								if (RandomUtils.Chance(50) && !gapTiles.Contains(coord))
									SetNeighbour(coord);
							}

							if (j < Spawnset.ArenaWidth - 1)
							{
								ArenaCoord coord = new ArenaCoord(j + 1, k);
								if (RandomUtils.Chance(50) && !gapTiles.Contains(coord))
									SetNeighbour(coord);
							}

							if (k > 0)
							{
								ArenaCoord coord = new ArenaCoord(j, k - 1);
								if (RandomUtils.Chance(50) && !gapTiles.Contains(coord))
									SetNeighbour(coord);
							}

							if (k < Spawnset.ArenaHeight - 1)
							{
								ArenaCoord coord = new ArenaCoord(j, k + 1);
								if (RandomUtils.Chance(50) && !gapTiles.Contains(coord))
									SetNeighbour(coord);
							}
						}
					}
				}
			}

			return tiles;

			void SetNeighbour(ArenaCoord coord)
			{
				gapTiles.Add(coord);
				tiles[coord.X, coord.Y] = TileUtils.VoidDefault;
			}
		}
	}
}