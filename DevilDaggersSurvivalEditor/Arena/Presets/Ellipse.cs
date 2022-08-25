using DevilDaggersSurvivalEditor.Core;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class Ellipse : AbstractEllipseArena
{
	public float Height { get; set; }

	public override float[,] GetTiles()
	{
		float[,] tiles = CreateArenaArray();

		for (int i = 0; i < Spawnset.ArenaDimension; i++)
		{
			for (int j = 0; j < Spawnset.ArenaDimension; j++)
			{
				if (IsPointInEllipse(Spawnset.ArenaDimension / 2 + OffsetX, Spawnset.ArenaDimension / 2 + OffsetY, i, j, InnerRadius, OuterRadius, AngleInDegrees))
					tiles[i, j] = Height;
			}
		}

		return tiles;
	}
}
