using DevilDaggersSurvivalEditor.Code.Utils.Editor;

namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public class CageRectangular : AbstractRectangularArena
	{
		public float InsideHeight { get; set; }
		public float WallHeight { get; set; } = 8;

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();
			SetHeightGlobally(tiles, ArenaUtils.VoidDefault);

			for (int i = X1; i < X2; i++)
				for (int j = Y1; j < Y2; j++)
					tiles[i, j] = ((i == X1 || i == X2 - 1) && j >= Y1 && j <= Y2 - 1) || ((j == Y1 || j == Y2 - 1) && i >= X1 && i <= X2 - 1) ? WallHeight : InsideHeight;

			return tiles;
		}
	}
}