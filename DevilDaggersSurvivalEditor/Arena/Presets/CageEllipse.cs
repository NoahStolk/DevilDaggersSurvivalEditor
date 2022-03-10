using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class CageEllipse : AbstractEllipseArena
{
	private int _wallThickness = 1;

	public CageEllipse(int dimension)
		: base(dimension)
	{
	}

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

		for (int i = 0; i < Dimension; i++)
		{
			for (int j = 0; j < Dimension; j++)
			{
				if (IsPointInEllipse(Dimension / 2 + OffsetX, Dimension / 2 + OffsetY, i, j, InnerRadius, OuterRadius, AngleInDegrees))
					tiles[i, j] = InsideHeight;
				else if (IsPointInEllipse(Dimension / 2 + OffsetX, Dimension / 2 + OffsetY, i, j, InnerRadius + WallThickness, OuterRadius + WallThickness, AngleInDegrees))
					tiles[i, j] = WallHeight;
			}
		}

		return tiles;
	}
}
