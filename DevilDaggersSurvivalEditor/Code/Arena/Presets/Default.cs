using DevilDaggersCore.Spawnset;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.Code.Arena.Presets
{
	public class Default : AbstractArena
	{
		public override bool IsFull => true;

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			byte[] defaultArenaBuffer = new byte[Spawnset.ArenaBufferSize];
			using (FileStream fs = new FileStream("Content/survival", FileMode.Open, FileAccess.Read) { Position = Spawnset.SettingsBufferSize })
				fs.Read(defaultArenaBuffer, 0, Spawnset.ArenaBufferSize);

			for (int i = 0; i < defaultArenaBuffer.Length; i += 4)
			{
				int x = i / (Spawnset.ArenaWidth * 4);
				int y = i / 4 % Spawnset.ArenaHeight;
				tiles[x, y] = BitConverter.ToSingle(defaultArenaBuffer, i);
			}

			return tiles;
		}
	}
}