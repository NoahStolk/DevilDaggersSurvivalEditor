using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Code.CustomLeaderboards.MemoryHandling.Variables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace DevilDaggersSurvivalEditor.Code.CustomLeaderboards.MemoryHandling
{
	public sealed class Scanner
	{
		private const int Magic = 0x001F30C0;

		public Process Process { get; set; }
		public Memory Memory { get; private set; } = new Memory();

		public string SpawnsetHash { get; private set; } = string.Empty;

		public IntVariable PlayerID { get; private set; } = new IntVariable(Magic, 0x5C);
		public StringVariable Username { get; private set; } = new StringVariable(Magic, 0x60, 32);
		public FloatVariable Time { get; private set; } = new FloatVariable(Magic, 0x1A0);
		public IntVariable Gems { get; private set; } = new IntVariable(Magic, 0x1C0);
		public IntVariable Kills { get; private set; } = new IntVariable(Magic, 0x1BC);
		public IntVariable DeathType { get; private set; } = new IntVariable(Magic, 0x1C4);
		public IntVariable ShotsFired { get; private set; } = new IntVariable(Magic, 0x1B4);
		public IntVariable ShotsHit { get; private set; } = new IntVariable(Magic, 0x1B8);
		public IntVariable EnemiesAlive { get; private set; } = new IntVariable(Magic, 0x1FC);
		public BoolVariable IsAlive { get; private set; } = new BoolVariable(Magic, 0x1A4);
		public BoolVariable IsReplay { get; private set; } = new BoolVariable(Magic, 0x35D);

		public float[] LevelUpTimes { get; private set; } = new float[3] { 0, 0, 0 };

		public int LevelGems { get; private set; }
		public int Homing { get; private set; }

		public List<int> HomingLog { get; private set; } = new List<int>();

		private static readonly Lazy<Scanner> lazy = new Lazy<Scanner>(() => new Scanner());
		public static Scanner Instance => lazy.Value;

		private Scanner()
		{
		}

		public void FindWindow()
		{
			Process = ProcessUtils.GetDevilDaggersProcess();
		}

		private string CalculateCurrentSurvivalHash()
		{
			try
			{
				using (FileStream fs = new FileStream(Path.Combine(Path.GetDirectoryName(Process.MainModule.FileName), "dd", "survival"), FileMode.Open, FileAccess.Read))
				{
					if (Spawnset.TryParse(fs, out Spawnset spawnset))
						return spawnset.GetHashString();

					Program.App.LogError("Failed to calculate spawnset hash because the survival file could not be parsed.");
				}

				return string.Empty;
			}
			catch (Exception ex)
			{
				Program.App.LogError("Failed to calculate spawnset hash.", ex);

				return string.Empty;
			}
		}

		public void RestartScan()
		{
			SpawnsetHash = string.Empty;

			LevelUpTimes = new float[3] { 0, 0, 0 };
		}

		/// <summary>
		/// Used to set previous values for every <see cref="AbstractVariable{T}"/>. Must use the same order and logic as the <see cref="Scan"/> method.
		/// </summary>
		public void PreScan()
		{
			PlayerID.PreScan();
			Username.PreScan();

			IsReplay.PreScan();
			if (IsReplay.Value)
				return;

			IsAlive.PreScan();
			Time.PreScan();
			Kills.PreScan();
			Gems.PreScan();
			ShotsFired.PreScan();
			ShotsHit.PreScan();

			if (IsAlive.Value)
				EnemiesAlive.PreScan();

			if (!IsAlive.Value)
				DeathType.PreScan();
		}

		public void Scan()
		{
			try
			{
				// Always scan these values.
				PlayerID.Scan();
				Username.Scan();

				// Always calculate the spawnset in menu or lobby.
				// Otherwise you can first normally load a spawnset to set the hash, exit and load an empty spawnset in the menu/lobby, then during playing the empty spawnset change it back to the same original spawnset and upload a cheated score.
				if (Time.Value == 0 && Time.ValuePrevious == 0)
					SpawnsetHash = CalculateCurrentSurvivalHash();

				// Stop scanning if it is a replay.
				IsReplay.Scan();
				if (IsReplay.Value)
					return;

				IsAlive.Scan();
				Time.Scan();
				Kills.Scan();
				Gems.Scan();
				ShotsFired.Scan();
				ShotsHit.Scan();

				if (IsAlive.Value)
				{
					// Enemy count might increase on death, so only scan while player is alive.
					EnemiesAlive.Scan();

					// TODO: Clean up
					byte[] bytes = Memory.Read(Process.MainModule.BaseAddress + 0x001F8084, 4, out _);
					int ptr = AddressUtils.ToDec(AddressUtils.MakeAddress(bytes));
					bytes = Memory.Read(new IntPtr(ptr), 4, out _);
					ptr = AddressUtils.ToDec(AddressUtils.MakeAddress(bytes));
					bytes = Memory.Read(new IntPtr(ptr) + 0x218, 4, out _);
					LevelGems = BitConverter.ToInt32(bytes, 0);

					bytes = Memory.Read(new IntPtr(ptr) + 0x224, 4, out _);
					Homing = BitConverter.ToInt32(bytes, 0);
					HomingLog.Add(Homing);
					if (HomingLog.Count > 5)
						HomingLog.Remove(HomingLog[0]);

					if (LevelUpTimes[0] == 0 && LevelGems >= 10 && LevelGems < 70)
						LevelUpTimes[0] = Time.Value;
					if (LevelUpTimes[1] == 0 && LevelGems == 70)
						LevelUpTimes[1] = Time.Value;
					if (LevelUpTimes[2] == 0 && LevelGems == 71)
						LevelUpTimes[2] = Time.Value;
				}
				else
				{
					// Only scan death type when dead.
					DeathType.Scan();
				}

				if (string.IsNullOrEmpty(SpawnsetHash))
					SpawnsetHash = CalculateCurrentSurvivalHash();
			}
			catch (Exception ex)
			{
				Program.App.LogError("Scan failed", ex);
			}
		}

		public void PrepareUpload()
		{
			if (HomingLog.Count > 0)
				Homing = HomingLog[0];
			else
				Program.App.LogError("Homing log is empty");
		}
	}
}