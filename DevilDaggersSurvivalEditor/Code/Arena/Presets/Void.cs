namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class Void : AbstractArena
	{
		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			SetHeightGlobally(tiles, ArenaUtils.VoidDefault);

			return tiles;
		}
	}
}