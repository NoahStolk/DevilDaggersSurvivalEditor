namespace DevilDaggersSurvivalEditor.Presets
{
	public class ArenaRandom : ArenaAbstract
	{
		public float MinHeight { get; set; }
		public float MaxHeight { get; set; }

		public ArenaRandom(int x1, int y1, int x2, int y2, float minHeight, float maxHeight)
			: base(x1, y1, x2, y2)
		{
			MinHeight = minHeight;
			MaxHeight = maxHeight;
		}

		public override float[,] GetTiles()
		{
			throw new System.NotImplementedException();
		}
	}
}