using DevilDaggersCore.Spawnsets;
using DevilDaggersSurvivalEditor.User;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.Utils
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
					App.Instance.MainWindow!.UpdateWarningNoSurvivalFile();
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
				using (Stream stream = App.Assembly.GetManifestResourceStream("DevilDaggersSurvivalEditor.Content.survival") ?? throw new Exception("Could not retrieve default survival file resource stream."))
				{
					byte[] data = new byte[stream.Length];
					using (BinaryReader reader = new BinaryReader(stream))
						reader.Read(data, 0, data.Length);

					using FileStream fileStream = new FileStream(UserHandler.Instance.Settings.SurvivalFileLocation, FileMode.Create);
					fileStream.Write(data, 0, data.Length);
				}

				App.Instance.ShowMessage("Success", "Successfully restored 'survival' file.");
				App.Instance.MainWindow!.UpdateWarningNoSurvivalFile();
			}
			catch (Exception ex)
			{
				App.Instance.ShowError("Unexpected error", $"Error while trying to write file to {UserHandler.Instance.Settings.SurvivalFileLocation}.", ex);
			}
		}
	}
}