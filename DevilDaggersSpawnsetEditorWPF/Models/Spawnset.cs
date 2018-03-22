using DevilDaggersSpawnsetEditorWPF.Helpers;
using System;
using System.Collections.Generic;
using System.IO;

namespace DevilDaggersSpawnsetEditorWPF.Models
{
	public class Spawnset
	{
		public SortedDictionary<int, Spawn> spawns;
		public float[,] arenaTiles;
		public float shrinkStart;
		public float shrinkEnd;
		public float shrinkRate;
		public float brightness;

		public Spawnset()
		{
			spawns = new SortedDictionary<int, Spawn>();
			arenaTiles = new float[Settings.ARENA_WIDTH, Settings.ARENA_HEIGHT];
			shrinkStart = 50;
			shrinkEnd = 20;
			shrinkRate = 0.025f;
			brightness = 60;

			byte[] defaultArenaBuffer = new byte[Settings.ARENA_BUFFER_SIZE];
			FileStream fs = new FileStream("Content/V3_Sorath", FileMode.Open, FileAccess.Read)
			{
				Position = Settings.HEADER_BUFFER_SIZE
			};
			fs.Read(defaultArenaBuffer, 0, Settings.ARENA_BUFFER_SIZE);
			fs.Close();

			for (int i = 0; i < defaultArenaBuffer.Length; i += 4)
			{
				int x = (i) / (Settings.ARENA_WIDTH * 4);
				int y = ((i) / 4) % Settings.ARENA_HEIGHT;
				arenaTiles[x, y] = BitConverter.ToSingle(defaultArenaBuffer, i);
			}
		}

		public Spawnset(SortedDictionary<int, Spawn> spawns, float[,] arenaTiles, float shrinkStart, float shrinkEnd, float shrinkRate, float brightness)
		{
			this.spawns = spawns;
			this.arenaTiles = arenaTiles;
			this.shrinkStart = shrinkStart;
			this.shrinkEnd = shrinkEnd;
			this.shrinkRate = shrinkRate;
			this.brightness = brightness;
		}

		public byte[] GetBytes()
		{
			// Open the original spawnset file
			FileStream fs = new FileStream("Content/V3_Sorath", FileMode.Open, FileAccess.Read);

			// Set the file values for reading V3 spawnsets
			byte[] headerBuffer = new byte[Settings.HEADER_BUFFER_SIZE];
			byte[] arenaBuffer = new byte[Settings.ARENA_BUFFER_SIZE];
			byte[] spawnBuffer = new byte[40 + spawns.Count * 28];

			// Read the file and write the data into the buffers, then close the file since we do not need it anymore
			fs.Read(headerBuffer, 0, Settings.HEADER_BUFFER_SIZE);
			fs.Read(arenaBuffer, 0, Settings.ARENA_BUFFER_SIZE);
			fs.Read(spawnBuffer, 0, 40);

			fs.Close();

			// Get the settings bytes and copy them into the header buffer
			byte[] shrinkEndBytes = BitConverter.GetBytes(shrinkEnd);
			byte[] shrinkStartBytes = BitConverter.GetBytes(shrinkStart);
			byte[] shrinkRateBytes = BitConverter.GetBytes(shrinkRate);
			byte[] brightnessBytes = BitConverter.GetBytes(brightness);

			for (int i = 0; i < shrinkEndBytes.Length; i++)
				headerBuffer[8 + i] = shrinkEndBytes[i];
			for (int i = 0; i < shrinkStartBytes.Length; i++)
				headerBuffer[12 + i] = shrinkStartBytes[i];
			for (int i = 0; i < shrinkRateBytes.Length; i++)
				headerBuffer[16 + i] = shrinkRateBytes[i];
			for (int i = 0; i < brightnessBytes.Length; i++)
				headerBuffer[20 + i] = brightnessBytes[i];
			
			// Get the arena bytes and copy them into the arena buffer
			for (int i = 0; i < arenaBuffer.Length; i += 4)
			{
				int x = i / (Settings.ARENA_WIDTH * 4);
				int y = (i / 4) % Settings.ARENA_HEIGHT;

				byte[] tileBytes = BitConverter.GetBytes(arenaTiles[x, y]);

				for (int j = 0; j < tileBytes.Length; j++)
					arenaBuffer[i + j] = tileBytes[j];
			}

			byte[] spawnCountBytes = BitConverter.GetBytes(spawns.Count);

			for (int i = 0; i < spawnCountBytes.Length; i++)
				spawnBuffer[36 + i] = spawnCountBytes[i];

			foreach (KeyValuePair<int, Spawn> kvp in spawns)
			{
				int enemyType = -1;
				for (int i = 0; i < GameHelper.enemies.Count-1; i++)
				{
					if (kvp.Value.enemy == GameHelper.enemies[i])
					{
						enemyType = i;
						break;
					}
				}
				byte[] enemyBytes = BitConverter.GetBytes(enemyType);
				for (int i = 0; i < enemyBytes.Length; i++)
					spawnBuffer[40 + kvp.Key * 28 + i] = enemyBytes[i];

				byte[] delayBytes = BitConverter.GetBytes((float)kvp.Value.delay);
				for (int i = 0; i < delayBytes.Length; i++)
					spawnBuffer[40 + kvp.Key * 28 + 4 + i] = delayBytes[i];
			}

			byte[] fileBuffer = new byte[headerBuffer.Length + arenaBuffer.Length + spawnBuffer.Length];

			Buffer.BlockCopy(headerBuffer, 0, fileBuffer, 0, headerBuffer.Length);
			Buffer.BlockCopy(arenaBuffer, 0, fileBuffer, headerBuffer.Length, arenaBuffer.Length);
			Buffer.BlockCopy(spawnBuffer, 0, fileBuffer, headerBuffer.Length + arenaBuffer.Length, spawnBuffer.Length);

			return fileBuffer;
		}
		
		/// <summary>
		/// Tries to parse the contents of a spawnset file into a Spawnset instance.
		/// This only works for V3 spawnsets.
		/// </summary>
		/// <param name="filePath">The path to the spawnset file.</param>
		/// <returns>The <see cref="Spawnset">Spawnset</see>.</returns>
		public static bool TryParse(string filePath, out Spawnset spawnset)
		{
			try
			{
				// Open the spawnset file
				FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

				// Set the file values for reading V3 spawnsets
				int spawnBufferSize = (int)fs.Length - (Settings.HEADER_BUFFER_SIZE + Settings.ARENA_BUFFER_SIZE);
				byte[] headerBuffer = new byte[Settings.HEADER_BUFFER_SIZE];
				byte[] arenaBuffer = new byte[Settings.ARENA_BUFFER_SIZE];
				byte[] spawnBuffer = new byte[spawnBufferSize];

				// Read the file and write the data into the buffers, then close the file since we do not need it anymore
				fs.Read(headerBuffer, 0, Settings.HEADER_BUFFER_SIZE);
				fs.Read(arenaBuffer, 0, Settings.ARENA_BUFFER_SIZE);
				fs.Read(spawnBuffer, 0, spawnBufferSize);

				fs.Close();

				// Set the header values
				float shrinkEnd = BitConverter.ToSingle(headerBuffer, 8);
				float shrinkStart = BitConverter.ToSingle(headerBuffer, 12);
				float shrinkRate = BitConverter.ToSingle(headerBuffer, 16);
				float brightness = BitConverter.ToSingle(headerBuffer, 20);

				// Set the arena values
				float[,] arenaTiles = new float[Settings.ARENA_WIDTH, Settings.ARENA_HEIGHT];
				for (int i = 0; i < arenaBuffer.Length; i += 4)
				{
					int x = i / (Settings.ARENA_WIDTH * 4);
					int y = (i / 4) % Settings.ARENA_HEIGHT;
					arenaTiles[x, y] = BitConverter.ToSingle(arenaBuffer, i);
				}

				// Set the spawn values
				SortedDictionary<int, Spawn> spawns = new SortedDictionary<int, Spawn>();
				int spawnIndex = 0;

				int bytePosition = 40;
				while (bytePosition < spawnBufferSize)
				{
					int enemyType = BitConverter.ToInt32(spawnBuffer, bytePosition);
					bytePosition += 4;
					float delay = BitConverter.ToSingle(spawnBuffer, bytePosition);
					bytePosition += 24;

					// Disable the loop for all previous spawns when we reach an empty spawn
					// The empty spawn is part of the new loop (until we find another empty spawn)
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