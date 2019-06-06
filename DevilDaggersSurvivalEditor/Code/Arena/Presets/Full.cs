using DevilDaggersSurvivalEditor.Code.Utils;
using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class Full : AbstractArena
	{
		private float height;

		public float Height
		{
			get => height;
			set => height = MathUtils.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public override bool IsFull => true;

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			SetHeightGlobally(tiles, Height);

			return tiles;
		}
	}
}