namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public class Cage : AbstractArena
	{
		public float InsideHeight { get; set; }
		public float WallHeight { get; set; }

		public Cage(int x1, int y1, int x2, int y2, float insideHeight, float wallHeight)
			: base(x1, y1, x2, y2)
		{
			InsideHeight = insideHeight;
			WallHeight = wallHeight;
		}

		public override float[,] GetTiles()
		{
			throw new System.NotImplementedException();
		}
	}
}