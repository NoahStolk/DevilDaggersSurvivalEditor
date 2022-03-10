using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class Hill : AbstractEllipseArena
{
	public Hill(int dimension)
		: base(dimension)
	{
	}

	public float StartHeight { get; set; }

	public float EndHeight { get; set; } = 8;

	public override float[,] GetTiles()
	{
		float[,] tiles = CreateArenaArray();

		ArenaCoord center = new(Dimension / 2 + OffsetX, Dimension / 2 + OffsetY);

		for (int i = 0; i < Dimension; i++)
		{
			for (int j = 0; j < Dimension; j++)
			{
				if (IsPointInEllipse(center.X, center.Y, i, j, InnerRadius, OuterRadius, AngleInDegrees))
				{
					int deltaX = i - center.X;
					int deltaY = j - center.Y;
					float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
					tiles[i, j] = Lerp(StartHeight, EndHeight, 1 - distance / OuterRadius);
				}
			}
		}

		return tiles;

		static float Lerp(float value1, float value2, float amount)
			=> value1 + (value2 - value1) * amount;
	}
}
