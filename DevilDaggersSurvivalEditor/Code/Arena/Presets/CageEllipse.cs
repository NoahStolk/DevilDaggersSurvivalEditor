using DevilDaggersCore.Spawnset;
using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class CageEllipse : AbstractEllipseArena
	{
		private float insideHeight;
		private float wallHeight = 8;

		public float InsideHeight
		{
			get => insideHeight;
			set => insideHeight = MathUtils.Clamp(value, ArenaUtils.TileMin, ArenaUtils.TileMax);
		}
		public float WallHeight
		{
			get => wallHeight;
			set => wallHeight = MathUtils.Clamp(value, ArenaUtils.TileMin, ArenaUtils.TileMax);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					tiles[i, j] = IsPointInEllipse(Spawnset.ArenaWidth / 2 + OffsetX, Spawnset.ArenaHeight / 2 + OffsetY, i, j, InnerRadius, OuterRadius, AngleInDegrees) ? InsideHeight : WallHeight;

			return tiles;
		}
	}
}