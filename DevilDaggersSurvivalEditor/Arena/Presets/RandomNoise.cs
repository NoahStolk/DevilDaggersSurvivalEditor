using DevilDaggersSurvivalEditor.Utils;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class RandomNoise : AbstractRectangularArena
{
	public float MinHeight { get; set; }

	public float MaxHeight { get; set; } = 16;

	public override float[,] GetTiles()
	{
		float[,] tiles = CreateArenaArray();

		for (int i = X1; i < X2; i++)
		{
			for (int j = Y1; j < Y2; j++)
				tiles[i, j] = RandomUtils.RandomFloat(MinHeight, MaxHeight);
		}

		return tiles;
	}
}
