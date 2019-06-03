using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class Rectangular : AbstractRectangularArena
	{
		private float height;

		public float Height
		{
			get => height;
			set => height = MathUtils.Clamp(value, ArenaCoord.TileMin, ArenaCoord.TileMax);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();
			SetHeightGlobally(tiles, ArenaCoord.VoidDefault);

			for (int i = X1; i < X2; i++)
				for (int j = Y1; j < Y2; j++)
					tiles[i, j] = Height;

			return tiles;
		}
	}
}