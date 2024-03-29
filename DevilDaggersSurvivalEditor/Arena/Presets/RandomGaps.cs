using DevilDaggersSurvivalEditor.Core;
using DevilDaggersSurvivalEditor.Utils;
using System;
using System.Collections.Generic;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class RandomGaps : AbstractRectangularArena
{
	private int _amount = 5;
	private int _iterations = 2;

	public float Height { get; set; }

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

		List<ArenaCoord> gapTiles = new();

		for (int i = 0; i < Amount; i++)
		{
			ArenaCoord coord = new(RandomUtils.RandomInt(X1, X2), RandomUtils.RandomInt(Y1, Y2));
			gapTiles.Add(coord);
			tiles[coord.X, coord.Y] = TileUtils.VoidDefault;
		}

		for (int i = 0; i < Iterations; i++)
		{
			for (int j = X1; j < X2; j++)
			{
				for (int k = Y1; k < Y2; k++)
				{
					if (gapTiles.Contains(new(j, k)))
					{
						if (j > 0)
						{
							ArenaCoord coord = new(j - 1, k);
							if (RandomUtils.Chance(50) && !gapTiles.Contains(coord))
								SetNeighbour(coord);
						}

						if (j < Spawnset.ArenaDimension - 1)
						{
							ArenaCoord coord = new(j + 1, k);
							if (RandomUtils.Chance(50) && !gapTiles.Contains(coord))
								SetNeighbour(coord);
						}

						if (k > 0)
						{
							ArenaCoord coord = new(j, k - 1);
							if (RandomUtils.Chance(50) && !gapTiles.Contains(coord))
								SetNeighbour(coord);
						}

						if (k < Spawnset.ArenaDimension - 1)
						{
							ArenaCoord coord = new(j, k + 1);
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
