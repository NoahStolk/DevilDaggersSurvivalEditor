namespace DevilDaggersSpawnsetEditorWPF.Presets
{
	public class ArenaRectangular : Arena
	{
		public float height;

		public ArenaRectangular(int x1, int y1, int x2, int y2, float height)
			: base(x1, y1, x2, y2)
		{
			this.height = height;
		}
	}
}