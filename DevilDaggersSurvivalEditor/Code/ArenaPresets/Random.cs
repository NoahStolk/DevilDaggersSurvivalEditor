using NetBase.Utils;

namespace DevilDaggersSurvivalEditor.Code.ArenaPresets
{
	public class Random : AbstractRectangularArena
	{
		public float MinHeight { get; set; }
		public float MaxHeight { get; set; }

		public override float[,] GetTiles()
		{
			float[,] tiles = VoidArena();

			for (int i = X1; i < X2; i++)
				for (int j = Y1; j < Y2; j++)
					tiles[i, j] = RandomUtils.RandomFloat(MinHeight, MaxHeight);

			return tiles;
		}
	}
}