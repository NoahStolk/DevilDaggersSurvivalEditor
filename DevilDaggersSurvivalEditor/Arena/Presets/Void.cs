using DevilDaggersSurvivalEditor.Utils;

namespace DevilDaggersSurvivalEditor.Arena.Presets
{
	public class Void : AbstractArena
	{
		public override bool IsFull => true;

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			SetHeightGlobally(tiles, TileUtils.VoidDefault);

			return tiles;
		}
	}
}