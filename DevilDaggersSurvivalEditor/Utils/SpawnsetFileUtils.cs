using DevilDaggersInfo.Core.Spawnset;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.Utils;

public static class SpawnsetFileUtils
{
	public static bool TryWriteSpawnsetToFile(SpawnsetBinary spawnset, string destinationPath)
	{
		try
		{
			File.WriteAllBytes(destinationPath, spawnset.ToBytes());
			return true;
		}
		catch (Exception ex)
		{
			App.Instance.ShowError("Unexpected error", $"Error while trying to write file to {destinationPath}.", ex);
			return false;
		}
	}
}
