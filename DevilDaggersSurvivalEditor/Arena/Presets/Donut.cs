using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class Donut : AbstractEllipseArena
{
	private int _holeOffsetX;
	private int _holeOffsetY;
	private float _holeInnerRadius = 5;
	private float _holeOuterRadius = 5;
	private float _holeAngleInDegrees;

	public Donut(int dimension)
		: base(dimension)
	{
	}

	public float Height { get; set; }

	public int HoleOffsetX
	{
		get => _holeOffsetX;
		set => _holeOffsetX = Math.Clamp(value, -Dimension, Dimension);
	}

	public int HoleOffsetY
	{
		get => _holeOffsetY;
		set => _holeOffsetY = Math.Clamp(value, -Dimension, Dimension);
	}

	public float HoleInnerRadius
	{
		get => _holeInnerRadius;
		set => _holeInnerRadius = Math.Clamp(value, 1, 100);
	}

	public float HoleOuterRadius
	{
		get => _holeOuterRadius;
		set => _holeOuterRadius = Math.Clamp(value, 1, 100);
	}

	public float HoleAngleInDegrees
	{
		get => _holeAngleInDegrees;
		set => _holeAngleInDegrees = value % 360;
	}

	public override float[,] GetTiles()
	{
		float[,] tiles = CreateArenaArray();

		for (int i = 0; i < Dimension; i++)
		{
			for (int j = 0; j < Dimension; j++)
			{
				if (IsPointInEllipse(Dimension / 2 + OffsetX, Dimension / 2 + OffsetY, i, j, InnerRadius, OuterRadius, AngleInDegrees) &&
					!IsPointInEllipse(Dimension / 2 + HoleOffsetX, Dimension / 2 + HoleOffsetY, i, j, HoleInnerRadius, HoleOuterRadius, HoleAngleInDegrees))
				{
					tiles[i, j] = Height;
				}
			}
		}

		return tiles;
	}
}
