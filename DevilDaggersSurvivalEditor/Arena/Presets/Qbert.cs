using DevilDaggersSurvivalEditor.Core;
using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class Qbert : AbstractRectangularArena
{
	private int _offsetX;
	private int _offsetY;

	public int OffsetX
	{
		get => _offsetX;
		set => _offsetX = Math.Clamp(value, -Spawnset.ArenaDimension, Spawnset.ArenaDimension);
	}

	public int OffsetY
	{
		get => _offsetY;
		set => _offsetY = Math.Clamp(value, -Spawnset.ArenaDimension, Spawnset.ArenaDimension);
	}

	public float StartHeight { get; set; } = -1;

	public float EndHeight { get; set; } = 17;

	public override float[,] GetTiles()
	{
		float[,] tiles = CreateArenaArray();

		float stepX = (StartHeight - EndHeight) / (X2 - X1 - 1);
		float stepY = (StartHeight - EndHeight) / (Y2 - Y1 - 1);
		for (int i = X1; i < X2; i++)
		{
			for (int j = Y1; j < Y2; j++)
				tiles[i + OffsetX, j + OffsetY] = EndHeight + (Math.Abs(i - Spawnset.ArenaDimension / 2) * stepX + Math.Abs(j - Spawnset.ArenaDimension / 2) * stepY);
		}

		return tiles;
	}
}
