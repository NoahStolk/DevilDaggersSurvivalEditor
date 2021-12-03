using DevilDaggersCore.Spawnsets;
using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class Donut : AbstractEllipseArena
{
	private int _holeOffsetX;
	private int _holeOffsetY;
	private float _holeInnerRadius = 5;
	private float _holeOuterRadius = 5;
	private float _holeAngleInDegrees;

	public float Height { get; set; }

	public int HoleOffsetX
	{
		get => _holeOffsetX;
		set => _holeOffsetX = Math.Clamp(value, -Spawnset.ArenaWidth, Spawnset.ArenaWidth);
	}

	public int HoleOffsetY
	{
		get => _holeOffsetY;
		set => _holeOffsetY = Math.Clamp(value, -Spawnset.ArenaHeight, Spawnset.ArenaHeight);
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

		for (int i = 0; i < Spawnset.ArenaWidth; i++)
		{
			for (int j = 0; j < Spawnset.ArenaHeight; j++)
			{
				if (IsPointInEllipse(Spawnset.ArenaWidth / 2 + OffsetX, Spawnset.ArenaHeight / 2 + OffsetY, i, j, InnerRadius, OuterRadius, AngleInDegrees) &&
					!IsPointInEllipse(Spawnset.ArenaWidth / 2 + HoleOffsetX, Spawnset.ArenaHeight / 2 + HoleOffsetY, i, j, HoleInnerRadius, HoleOuterRadius, HoleAngleInDegrees))
				{
					tiles[i, j] = Height;
				}
			}
		}

		return tiles;
	}
}
