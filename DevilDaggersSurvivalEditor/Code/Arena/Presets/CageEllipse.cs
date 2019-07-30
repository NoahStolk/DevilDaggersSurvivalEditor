using DevilDaggersCore.Spawnsets;
using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class CageEllipse : AbstractEllipseArena
	{
		private float insideHeight;
		private float wallHeight = 8;
		private int wallThickness = 1;

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
		public int WallThickness
		{
			get => wallThickness;
			set => wallThickness = MathUtils.Clamp(value, 1, 20);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					if (IsPointInEllipse(Spawnset.ArenaWidth / 2 + OffsetX, Spawnset.ArenaHeight / 2 + OffsetY, i, j, InnerRadius, OuterRadius))
						tiles[i, j] = InsideHeight;
					else if (IsPointInEllipse(Spawnset.ArenaWidth / 2 + OffsetX, Spawnset.ArenaHeight / 2 + OffsetY, i, j, InnerRadius + WallThickness, OuterRadius + WallThickness))
						tiles[i, j] = WallHeight;

			return tiles;
		}
	}
}