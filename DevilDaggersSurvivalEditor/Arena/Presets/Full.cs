namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class Full : AbstractArena
{
	public float Height { get; set; }

	public override bool IsFull => true;

	public override float[,] GetTiles()
	{
		float[,] tiles = CreateArenaArray();

		SetHeightGlobally(tiles, Height);

		return tiles;
	}
}
