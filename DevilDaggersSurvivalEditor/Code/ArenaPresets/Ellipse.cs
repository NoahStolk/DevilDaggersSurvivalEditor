using DevilDaggersCore.Spawnset;
using DevilDaggersSurvivalEditor.Code.Utils.Editor;

namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public class Ellipse : AbstractEllipseArena
	{
		public float Height { get; set; }

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
					tiles[i, j] = IsPointInEllipse(Spawnset.ArenaWidth / 2 + OffsetX, Spawnset.ArenaHeight / 2 + OffsetY, i, j, InnerRadius, OuterRadius, AngleInDegrees) ? Height : ArenaUtils.VoidDefault;

			return tiles;
		}
	}
}