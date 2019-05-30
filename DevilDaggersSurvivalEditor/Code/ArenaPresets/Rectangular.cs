namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public class Rectangular : AbstractArena
	{
		public float Height { get; set; }

		public Rectangular(int x1, int y1, int x2, int y2, float height)
			: base(x1, y1, x2, y2)
		{
			Height = height;
		}

		public override float[,] GetTiles()
		{
			throw new System.NotImplementedException();
		}
	}
}