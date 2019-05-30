namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public class Pyramid : AbstractArena
	{
		public float StartHeight { get; set; }
		public float EndHeight { get; set; }

		public Pyramid(int x1, int y1, int x2, int y2, float startHeight, float endHeight)
			: base(x1, y1, x2, y2)
		{
			StartHeight = startHeight;
			EndHeight = endHeight;
		}

		public override float[,] GetTiles()
		{
			throw new System.NotImplementedException();
		}
	}
}