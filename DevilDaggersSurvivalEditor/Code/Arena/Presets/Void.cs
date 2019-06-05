using DevilDaggersSurvivalEditor.Code.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class Void : AbstractArena
	{
		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			SetHeightGlobally(tiles, TileUtils.VoidDefault);

			return tiles;
		}
	}
}