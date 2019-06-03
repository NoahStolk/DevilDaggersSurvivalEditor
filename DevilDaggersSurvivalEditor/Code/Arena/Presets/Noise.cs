using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class Noise : AbstractRectangularArena
	{
		private float minHeight;
		private float maxHeight = 16;

		public float MinHeight
		{
			get => minHeight;
			set => minHeight = MathUtils.Clamp(value, ArenaCoord.TileMin, ArenaCoord.TileMax);
		}
		public float MaxHeight
		{
			get => maxHeight;
			set => maxHeight = MathUtils.Clamp(value, ArenaCoord.TileMin, ArenaCoord.TileMax);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();
			SetHeightGlobally(tiles, ArenaCoord.VoidDefault);

			for (int i = X1; i < X2; i++)
				for (int j = Y1; j < Y2; j++)
					tiles[i, j] = RandomUtils.RandomFloat(MinHeight, MaxHeight);

			return tiles;
		}
	}
}