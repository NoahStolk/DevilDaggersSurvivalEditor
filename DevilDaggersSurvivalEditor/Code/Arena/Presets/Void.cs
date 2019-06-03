namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class Void : AbstractArena
	{
		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			SetHeightGlobally(tiles, ArenaCoord.VoidDefault);

			return tiles;
		}
	}
}