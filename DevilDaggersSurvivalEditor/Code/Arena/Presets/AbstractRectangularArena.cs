using DevilDaggersCore.Spawnsets;
using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	internal abstract class AbstractRectangularArena : AbstractArena
	{
		private int x1 = 15;
		private int y1 = 15;
		private int x2 = Spawnset.ArenaWidth - 15;
		private int y2 = Spawnset.ArenaHeight - 15;

		internal int X1 { get => x1; set { x1 = MathUtils.Clamp(value, 0, Spawnset.ArenaWidth); } }
		internal int Y1 { get => y1; set { y1 = MathUtils.Clamp(value, 0, Spawnset.ArenaHeight); } }
		internal int X2 { get => x2; set { x2 = MathUtils.Clamp(value, 0, Spawnset.ArenaWidth); } }
		internal int Y2 { get => y2; set { y2 = MathUtils.Clamp(value, 0, Spawnset.ArenaHeight); } }

		internal override bool IsFull => false;
	}
}