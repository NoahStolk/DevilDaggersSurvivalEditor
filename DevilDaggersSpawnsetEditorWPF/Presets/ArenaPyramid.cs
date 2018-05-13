namespace DevilDaggersSpawnsetEditorWPF.Presets
{
	public class ArenaPyramid : Arena
	{
		public float startHeight;
		public float endHeight;

		public ArenaPyramid(int x1, int y1, int x2, int y2, float startHeight, float endHeight)
			: base(x1, y1, x2, y2)
		{
			this.startHeight = startHeight;
			this.endHeight = endHeight;
		}
	}
}