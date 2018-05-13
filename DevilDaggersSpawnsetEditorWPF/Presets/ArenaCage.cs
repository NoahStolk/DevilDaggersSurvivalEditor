namespace DevilDaggersSpawnsetEditorWPF.Presets
{
	public class ArenaCage : Arena
	{
		public float insideHeight;
		public float wallHeight;

		public ArenaCage(int x1, int y1, int x2, int y2, float insideHeight, float wallHeight)
			: base(x1, y1, x2, y2)
		{
			this.insideHeight = insideHeight;
			this.wallHeight = wallHeight;
		}
	}
}