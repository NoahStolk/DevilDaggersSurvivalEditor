using DevilDaggersSurvivalEditor.Core;
using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class Pyramid : AbstractArena
{
	private int _offsetX;
	private int _offsetY;
	private int _size = 16;

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

	public float StartHeight { get; set; }

	public float EndHeight { get; set; } = 6;

	public int Size
	{
		get => _size;
		set => _size = Math.Clamp(value, 2, Spawnset.ArenaDimension);
	}

	public override bool IsFull => false;

	public override float[,] GetTiles()
	{
		float[,] tiles = CreateArenaArray();

		int halfSize = Size / 2;
		for (int i = 0; i < halfSize; i++)
		{
			int coord = Spawnset.ArenaDimension / 2 - halfSize + i;

			for (int j = 0; j <= (Spawnset.ArenaDimension / 2 - coord) * 2; j++)
			{
				float height = StartHeight + i / (float)halfSize * (EndHeight - StartHeight);

				tiles[coord + OffsetX, coord + j + OffsetY] = height;
				tiles[coord + j + OffsetX, coord + OffsetY] = height;

				tiles[Spawnset.ArenaDimension - 1 - coord + OffsetX, coord + j + OffsetY] = height;
				tiles[coord + j + OffsetX, Spawnset.ArenaDimension - 1 - coord + OffsetY] = height;
			}

			tiles[Spawnset.ArenaDimension / 2 + OffsetX, Spawnset.ArenaDimension / 2 + OffsetY] = EndHeight;
		}

		return tiles;
	}
}
