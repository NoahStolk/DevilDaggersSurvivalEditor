using DevilDaggersInfo.Core.Spawnset;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.Arena.Presets;

public class Default : AbstractArena
{
	public Default()
		: base(51)
	{
	}

	public override bool IsFull => true;

	public override float[,] GetTiles()
	{
		float[,] tiles = CreateArenaArray();

		byte[] defaultArenaBuffer = new byte[Dimension * Dimension * sizeof(float)];

		using (Stream stream = App.Assembly.GetManifestResourceStream("DevilDaggersSurvivalEditor.Content.survival") ?? throw new("Could not retrieve default survival file resource stream."))
		using (BinaryReader reader = new(stream))
		{
			reader.BaseStream.Seek(SpawnsetBinary.HeaderBufferSize, SeekOrigin.Begin);
			reader.Read(defaultArenaBuffer, 0, defaultArenaBuffer.Length);
		}

		for (int i = 0; i < defaultArenaBuffer.Length; i += 4)
		{
			int x = i / (Dimension * 4);
			int y = i / 4 % Dimension;
			tiles[x, y] = BitConverter.ToSingle(defaultArenaBuffer, i);
		}

		return tiles;
	}
}
