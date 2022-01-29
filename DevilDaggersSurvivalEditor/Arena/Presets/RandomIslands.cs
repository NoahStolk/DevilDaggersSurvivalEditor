using DevilDaggersSurvivalEditor.Core;
using DevilDaggersSurvivalEditor.Utils;
using System;
using System.Collections.Generic;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class RandomIslands : AbstractRectangularArena
{
	private int _amount = 5;
	private int _iterations = 2;
	private float _steepness = 0.1f;

	public float MinHeight { get; set; }

	public float MaxHeight { get; set; } = 5;

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

	public float Steepness
	{
		get => _steepness;
		set => _steepness = Math.Clamp(value, -5, 5);
	}

	public override float[,] GetTiles()
	{
		float[,] tiles = CreateArenaArray();

		List<ArenaCoord> islandTiles = new();

		for (int i = 0; i < Amount; i++)
		{
			ArenaCoord coord = new(RandomUtils.RandomInt(X1, X2), RandomUtils.RandomInt(Y1, Y2));
			islandTiles.Add(coord);
			tiles[coord.X, coord.Y] = RandomUtils.RandomFloat(MinHeight, MaxHeight);
		}

		for (int i = 0; i < Iterations; i++)
		{
			for (int j = X1; j < X2; j++)
			{
				for (int k = Y1; k < Y2; k++)
				{
					if (islandTiles.Contains(new(j, k)))
					{
						float height = tiles[j, k];

						if (j > 0)
						{
							ArenaCoord coord = new(j - 1, k);
							if (RandomUtils.Chance(50) && !islandTiles.Contains(coord))
								SetNeighbour(coord, height);
						}

						if (j < Spawnset.ArenaWidth - 1)
						{
							ArenaCoord coord = new(j + 1, k);
							if (RandomUtils.Chance(50) && !islandTiles.Contains(coord))
								SetNeighbour(coord, height);
						}

						if (k > 0)
						{
							ArenaCoord coord = new(j, k - 1);
							if (RandomUtils.Chance(50) && !islandTiles.Contains(coord))
								SetNeighbour(coord, height);
						}

						if (k < Spawnset.ArenaHeight - 1)
						{
							ArenaCoord coord = new(j, k + 1);
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
