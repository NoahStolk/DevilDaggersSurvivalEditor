using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class CageRectangular : AbstractRectangularArena
{
	private int _wallThickness = 1;

	public float InsideHeight { get; set; }

	public float WallHeight { get; set; } = 8;

	public int WallThickness
	{
		get => _wallThickness;
		set => _wallThickness = Math.Clamp(value, 1, 20);
	}

	public override float[,] GetTiles()
	{
		float[,] tiles = CreateArenaArray();

		for (int i = X1; i < X2; i++)
		{
			for (int j = Y1; j < Y2; j++)
			{
				bool x1 = i >= X1 && i < X1 + WallThickness;
				bool x2 = i >= X2 - WallThickness && i < X2;
				bool y1 = j >= Y1 && j < Y1 + WallThickness;
				bool y2 = j >= Y2 - WallThickness && j < Y2;
				tiles[i, j] = x1 || x2 || y1 || y2 ? WallHeight : InsideHeight;
			}
		}

		return tiles;
	}
}
