﻿using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.Code.User;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.Code.Spawnsets
{
	public static class SpawnsetFileUtils
	{
		public static bool TryWriteSpawnsetToFile(Spawnset spawnset, string destinationPath)
		{
			try
			{
				if (spawnset.TryGetBytes(out byte[] bytes))
				{
					File.WriteAllBytes(destinationPath, bytes);
					App.Instance.MainWindow.UpdateWarningNoSurvivalFile();
					return true;
				}
				else
				{
					App.Instance.ShowError("Unexpected error", "Error while trying to convert spawnset to binary.");
					return false;
				}
			}
			catch (Exception ex)
			{
				App.Instance.ShowError("Unexpected error", $"Error while trying to write file to {destinationPath}.", ex);
				return false;
			}
		}

		public static void TryRestoreSurvivalFile()
		{
			try
			{
				using (Stream stream = App.Assembly.GetManifestResourceStream("DevilDaggersSurvivalEditor.Content.survival"))
				{
					byte[] data = new byte[stream.Length];
					using (BinaryReader reader = new BinaryReader(stream))
						reader.Read(data, 0, data.Length);

					using FileStream fileStream = new FileStream(UserHandler.Instance._settings.SurvivalFileLocation, FileMode.Create);
					fileStream.Write(data, 0, data.Length);
				}

				App.Instance.ShowMessage("Success", "Successfully restored 'survival' file.");
				App.Instance.MainWindow.UpdateWarningNoSurvivalFile();
			}
			catch (Exception ex)
			{
				App.Instance.ShowError("Unexpected error", $"Error while trying to write file to {UserHandler.Instance._settings.SurvivalFileLocation}.", ex);
			}
		}
	}
}