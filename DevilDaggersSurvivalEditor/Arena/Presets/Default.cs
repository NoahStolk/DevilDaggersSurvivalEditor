using DevilDaggersSurvivalEditor.Core;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class Default : AbstractArena
{
	public override bool IsFull => true;

	public override float[,] GetTiles()
	{
		float[,] tiles = CreateArenaArray();

		byte[] defaultArenaBuffer = new byte[Spawnset.ArenaBufferSize];

		using (Stream stream = App.Assembly.GetManifestResourceStream("DevilDaggersSurvivalEditor.Content.survival") ?? throw new("Could not retrieve default survival file resource stream."))
		using (BinaryReader reader = new(stream))
		{
			reader.BaseStream.Seek(Spawnset.HeaderBufferSize, SeekOrigin.Begin);
			reader.Read(defaultArenaBuffer, 0, Spawnset.ArenaBufferSize);
		}

		for (int i = 0; i < defaultArenaBuffer.Length; i += 4)
		{
			int x = i / (Spawnset.ArenaDimension * 4);
			int y = i / 4 % Spawnset.ArenaDimension;
			tiles[x, y] = BitConverter.ToSingle(defaultArenaBuffer, i);
		}

		return tiles;
	}
}
