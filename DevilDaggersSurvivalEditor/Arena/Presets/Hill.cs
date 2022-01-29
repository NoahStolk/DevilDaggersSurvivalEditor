using DevilDaggersSurvivalEditor.Core;
using System;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class Hill : AbstractEllipseArena
{
	public float StartHeight { get; set; }

	public float EndHeight { get; set; } = 8;

	public override float[,] GetTiles()
	{
		float[,] tiles = CreateArenaArray();

		ArenaCoord center = new(Spawnset.ArenaWidth / 2 + OffsetX, Spawnset.ArenaHeight / 2 + OffsetY);

		for (int i = 0; i < Spawnset.ArenaWidth; i++)
		{
			for (int j = 0; j < Spawnset.ArenaHeight; j++)
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
