using DevilDaggersCore.Spawnset;
using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public abstract class AbstractRectangularArena : AbstractArena
	{
		private int x1;
		private int y1;
		private int x2 = Spawnset.ArenaWidth;
		private int y2 = Spawnset.ArenaHeight;

		public int X1 { get => x1; set { x1 = MathUtils.Clamp(value, 0, Spawnset.ArenaWidth); } }
		public int Y1 { get => x2; set { y1 = MathUtils.Clamp(value, 0, Spawnset.ArenaHeight); } }
		public int X2 { get => y1; set { x2 = MathUtils.Clamp(value, 0, Spawnset.ArenaWidth); } }
		public int Y2 { get => y2; set { y2 = MathUtils.Clamp(value, 0, Spawnset.ArenaHeight); } }
	}
}