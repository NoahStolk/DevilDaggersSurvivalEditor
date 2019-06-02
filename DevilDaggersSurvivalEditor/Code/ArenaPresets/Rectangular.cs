using DevilDaggersSurvivalEditor.Code.Utils.Editor;

namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public class Rectangular : AbstractRectangularArena
	{
		public float Height { get; set; }

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();
			SetHeightGlobally(tiles, ArenaUtils.VoidDefault);

			for (int i = X1; i < X2; i++)
				for (int j = Y1; j < Y2; j++)
					tiles[i, j] = Height;

			return tiles;
		}
	}
}