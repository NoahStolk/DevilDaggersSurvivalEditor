using DevilDaggersCore.Spawnset;

namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public class CageEllipse : AbstractEllipseArena
	{
		public float InsideHeight { get; set; }
		public float WallHeight { get; set; } = 8;

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