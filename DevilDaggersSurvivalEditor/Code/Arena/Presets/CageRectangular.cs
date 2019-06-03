using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class CageRectangular : AbstractRectangularArena
	{
		private float insideHeight;
		private float wallHeight = 8;

		public float InsideHeight
		{
			get => insideHeight;
			set => insideHeight = MathUtils.Clamp(value, ArenaCoord.TileMin, ArenaCoord.TileMax);
		}
		public float WallHeight
		{
			get => wallHeight;
			set => wallHeight = MathUtils.Clamp(value, ArenaCoord.TileMin, ArenaCoord.TileMax);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();
			SetHeightGlobally(tiles, ArenaCoord.VoidDefault);

			for (int i = X1; i < X2; i++)
				for (int j = Y1; j < Y2; j++)
					tiles[i, j] = ((i == X1 || i == X2 - 1) && j >= Y1 && j <= Y2 - 1) || ((j == Y1 || j == Y2 - 1) && i >= X1 && i <= X2 - 1) ? WallHeight : InsideHeight;

			return tiles;
		}
	}
}