﻿using DevilDaggersCore.Spawnsets;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.Arena.Presets
{
	public class Default : AbstractArena
	{
		public override bool IsFull => true;

		public override float[,] GetTiles()
		{
			float[,] tiles = CreateArenaArray();

			byte[] defaultArenaBuffer = new byte[Spawnset.ArenaBufferSize];

			using (Stream stream = App.Assembly.GetManifestResourceStream("DevilDaggersSurvivalEditor.Content.survival") ?? throw new Exception("Could not retrieve resource stream."))
			using (BinaryReader reader = new BinaryReader(stream))
			{
				reader.BaseStream.Seek(Spawnset.HeaderBufferSize, SeekOrigin.Begin);
				reader.Read(defaultArenaBuffer, 0, Spawnset.ArenaBufferSize);
			}

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