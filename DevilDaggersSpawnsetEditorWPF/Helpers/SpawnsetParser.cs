using DevilDaggersSpawnsetEditorWPF.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace DevilDaggersSpawnsetEditorWPF.Helpers
{
	public static class SpawnsetParser
	{
		private const int HEADER_BUFFER_SIZE = 76;
		private const int ARENA_BUFFER_SIZE = 10404;

		public const int ARENA_WIDTH = 51;
		public const int ARENA_HEIGHT = 51;

		/// <summary>
		/// Tries to parse the contents of a spawnset file into a Spawnset instance.
		/// This only works for V3 spawnsets.
		/// </summary>
		/// <param name="path">The path to the spawnset file.</param>
		/// <returns>The <see cref="Spawnset">Spawnset</see>.</returns>
		public static bool TryParseFile(string path, out Spawnset spawnset)
		{
			try
			{
				// Open the spawnset file
				FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

				// Set the file values for reading V3 spawnsets
				int spawnBufferSize = (int)fs.Length - (HEADER_BUFFER_SIZE + ARENA_BUFFER_SIZE);
				byte[] headerBuffer = new byte[HEADER_BUFFER_SIZE];
				byte[] arenaBuffer = new byte[ARENA_BUFFER_SIZE];
				byte[] spawnBuffer = new byte[spawnBufferSize];

				// Read the file and write the data into the buffers, then close the file since we do not need it anymore
				fs.Read(headerBuffer, 0, HEADER_BUFFER_SIZE);
				fs.Read(arenaBuffer, 0, ARENA_BUFFER_SIZE);
				fs.Read(spawnBuffer, 0, spawnBufferSize);

				fs.Close();

				// Set the header values
				float shrinkEnd = BitConverter.ToSingle(headerBuffer, 8);
				float shrinkStart = BitConverter.ToSingle(headerBuffer, 12);
				float shrinkRate = BitConverter.ToSingle(headerBuffer, 16);
				float brightness = BitConverter.ToSingle(headerBuffer, 20);

				// Set the arena values
				float[,] arenaTiles = new float[ARENA_WIDTH, ARENA_HEIGHT];
				for (int i = 0; i < arenaBuffer.Length; i += 4)
				{
					int x = i / (ARENA_WIDTH * 4);
					int y = (i / 4) % ARENA_HEIGHT;
					arenaTiles[x, y] = BitConverter.ToSingle(arenaBuffer, i);
				}

				// Set the spawn values
				Dictionary<int, Spawn> spawns = new Dictionary<int, Spawn>();
				int spawnIndex = 0;

				int bytePosition = 0;
				while (bytePosition < spawnBufferSize)
				{
					int enemyType = BitConverter.ToInt32(spawnBuffer, bytePosition);
					bytePosition += 4;
					float delay = BitConverter.ToSingle(spawnBuffer, bytePosition);
					bytePosition += 24;

					if (enemyType == -1)
					{
						foreach (KeyValuePair<int, Spawn> kvp in spawns)
							kvp.Value.loop = false;
					}

					spawns.Add(spawnIndex, new Spawn(GameHelper.enemies[enemyType], delay, true));
					spawnIndex++;
				}

				// Set the spawnset
				spawnset = new Spawnset(spawns, arenaTiles, shrinkStart, shrinkEnd, shrinkRate, brightness);

				// Success
				return true;
			}
			catch (Exception)
			{
				// Set an empty spawnset
				spawnset = new Spawnset();

				// Failure
				return false;
			}
		}
	}
}