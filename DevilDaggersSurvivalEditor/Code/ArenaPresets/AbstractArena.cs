namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public abstract class AbstractArena
	{
		public int X1 { get; set; }
		public int Y1 { get; set; }
		public int X2 { get; set; }
		public int Y2 { get; set; }

		public AbstractArena(int x1, int y1, int x2, int y2)
		{
			X1 = x1;
			Y1 = y1;
			X2 = x2;
			Y2 = y2;
		}

		public abstract float[,] GetTiles();
	}
}