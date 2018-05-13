namespace DevilDaggersSpawnsetEditorWPF.Presets
{
	public class ArenaPyramid : Arena
	{
		public int x1;
		public int y1;
		public int x2;
		public int y2;
		public float startHeight;
		public float endHeight;

		public ArenaPyramid(int x1, int y1, int x2, int y2, float startHeight, float endHeight)
		{
			this.x1 = x1;
			this.y1 = y1;
			this.x2 = x2;
			this.y2 = y2;
			this.startHeight = startHeight;
			this.endHeight = endHeight;
		}
	}
}