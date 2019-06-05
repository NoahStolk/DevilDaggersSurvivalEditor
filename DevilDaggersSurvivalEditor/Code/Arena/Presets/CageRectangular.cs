using DevilDaggersSurvivalEditor.Code.Utils;
using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class CageRectangular : AbstractRectangularArena
	{
		private float insideHeight;
		private float wallHeight = 8;
		private int thickness = 1;

		public float InsideHeight
		{
			get => insideHeight;
			set => insideHeight = MathUtils.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}
		public float WallHeight
		{
			get => wallHeight;
			set => wallHeight = MathUtils.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}
		public int Thickness
		{
			get => thickness;
			set => thickness = MathUtils.Clamp(value, 1, 20);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();
			SetHeightGlobally(tiles, TileUtils.VoidDefault);

			for (int i = X1; i < X2; i++)
				for (int j = Y1; j < Y2; j++)
					tiles[i, j] = (i >= X1 && i < X1 + Thickness)
							   || (i >= X2 - Thickness && i < X2)
							   || (j >= Y1 && j < Y1 + Thickness)
							   || (j >= Y2 - Thickness && j < Y2)
								? WallHeight : InsideHeight;

			return tiles;
		}
	}
}