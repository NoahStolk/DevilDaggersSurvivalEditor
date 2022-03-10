namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class Ellipse : AbstractEllipseArena
{
	public Ellipse(int dimension)
		: base(dimension)
	{
	}

	public float Height { get; set; }

	public override float[,] GetTiles()
	{
		float[,] tiles = CreateArenaArray();

		for (int i = 0; i < Dimension; i++)
		{
			for (int j = 0; j < Dimension; j++)
			{
				if (IsPointInEllipse(Dimension / 2 + OffsetX, Dimension / 2 + OffsetY, i, j, InnerRadius, OuterRadius, AngleInDegrees))
					tiles[i, j] = Height;
			}
		}

		return tiles;
	}
}
