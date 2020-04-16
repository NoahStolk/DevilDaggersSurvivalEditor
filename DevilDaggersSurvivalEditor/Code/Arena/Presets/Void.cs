namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	internal class Void : AbstractArena
	{
		internal override bool IsFull => true;

		internal override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			SetHeightGlobally(tiles, TileUtils.VoidDefault);

			return tiles;
		}
	}
}