using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Utils;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class Ellipse : AbstractEllipseArena
	{
		private float _height;

		public float Height
		{
			get => _height;
			set => _height = MathUtils.Clamp(value, TileUtils.TileMin, TileUtils.TileMax);
		}

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			for (int i = 0; i < Spawnset.ArenaWidth; i++)
			{
				for (int j = 0; j < Spawnset.ArenaHeight; j++)
				{
					if (IsPointInEllipse(Spawnset.ArenaWidth / 2 + OffsetX, Spawnset.ArenaHeight / 2 + OffsetY, i, j, InnerRadius, OuterRadius, AngleInDegrees))
						tiles[i, j] = Height;
				}
			}

			return tiles;
		}
	}
}