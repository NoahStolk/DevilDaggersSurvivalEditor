using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class Full : AbstractArena
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

			SetHeightGlobally(tiles, Height);

			return tiles;
		}
	}
}