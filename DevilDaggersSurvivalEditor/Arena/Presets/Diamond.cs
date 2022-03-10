using DevilDaggersSurvivalEditor.Utils;
using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class Diamond : AbstractArena
{
	private int _diamondHalfWidth = 10;
	private int _diamondHalfHeight = 10;

	public Diamond(int dimension)
		: base(dimension)
	{
	}

	public int DiamondHalfWidth
	{
		get => _diamondHalfWidth;
		set => _diamondHalfWidth = Math.Clamp(value, 1, Dimension / 2);
	}

	public int DiamondHalfHeight
	{
		get => _diamondHalfHeight;
		set => _diamondHalfHeight = Math.Clamp(value, 1, Dimension / 2);
	}

	public float Height { get; set; }

	public override bool IsFull => false;

	public override float[,] GetTiles()
	{
		float[,] tiles = CreateArenaArray();

		for (int i = -_diamondHalfWidth; i <= _diamondHalfWidth; i++)
		{
			for (int j = -_diamondHalfHeight; j <= _diamondHalfHeight; j++)
			{
				int sum = Math.Abs(i) + Math.Abs(j);
				tiles[i + Dimension / 2, j + Dimension / 2] = sum > Math.Max(_diamondHalfWidth, _diamondHalfHeight) ? TileUtils.VoidDefault : Height;
			}
		}

		return tiles;
	}
}
