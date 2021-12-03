using DevilDaggersCore.Spawnsets;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class Ellipse : AbstractEllipseArena
{
	public float Height { get; set; }

	public override float[,] GetTiles()
	{
		float[,] tiles = CreateArenaArray();

		for (int i = 0; i < Spawnset.ArenaWidth; i++)
		{
			for (int j = 0; j < Spawnset.ArenaHeight; j++)
			{
				if (IsPointInEllipse(Spawnset.ArenaWidth / 2 + OffsetX, Spawnset.ArenaHeight / 2 + OffsetY, i, j, InnerRadius, OuterRadius, AngleInDegrees))
					tiles[i, j] = Height;
			}
		}

		return tiles;
	}
}
