namespace DevilDaggersSurvivalEditor.Presets
{
	public class ArenaRandom : Arena
	{
		public float minHeight;
		public float maxHeight;

		public ArenaRandom(int x1, int y1, int x2, int y2, float minHeight, float maxHeight)
			: base(x1, y1, x2, y2)
		{
			this.minHeight = minHeight;
			this.maxHeight = maxHeight;
		}
	}
}