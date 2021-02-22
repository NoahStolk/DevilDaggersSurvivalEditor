using DevilDaggersCore.Spawnsets;
using DevilDaggersCore.Wpf.Windows;
using DevilDaggersSurvivalEditor.Arena;
using DevilDaggersSurvivalEditor.User;
using DevilDaggersSurvivalEditor.Utils;
using Microsoft.Win32;
using System;
using System.IO;

namespace DevilDaggersSurvivalEditor.Spawnsets
{
	public sealed class SpawnsetHandler
	{
		private bool _hasUnsavedChanges;

		private static readonly Lazy<SpawnsetHandler> _lazy = new(() => new());

		private SpawnsetHandler()
		{
			Spawnset = new() { ArenaTiles = ArenaPresetHandler.Instance.DefaultPreset.GetTiles() };
		}

		public static SpawnsetHandler Instance => _lazy.Value;

		public Spawnset Spawnset { get; set; }

		public bool HasUnsavedChanges
		{
			get => _hasUnsavedChanges;
			set
			{
				_hasUnsavedChanges = value;
				App.Instance.UpdateMainWindowTitle();
			}
		}

		public string SpawnsetFileName { get; private set; } = "(new spawnset)";
		public string SpawnsetFileLocation { get; private set; } = string.Empty;

		public void UpdateSpawnsetState(string name, string fileLocation)
		{
			HasUnsavedChanges = false;

			SpawnsetFileName = name;
			SpawnsetFileLocation = fileLocation;

			App.Instance.UpdateMainWindowTitle();
		}

		public void ProceedWithUnsavedChanges()
		{
			if (!HasUnsavedChanges)
				return;

			ConfirmWindow confirmWindow = new("Save changes?", "The current spawnset has unsaved changes. Save before proceeding?", false);
			confirmWindow.ShowDialog();

			if (confirmWindow.IsConfirmed)
				FileSave();
		}

		public void FileSave()
		{
			if (File.Exists(SpawnsetFileLocation))
			{
				if (SpawnsetFileUtils.TryWriteSpawnsetToFile(Spawnset, SpawnsetFileLocation))
					HasUnsavedChanges = false;
			}
			else
			{
				FileSaveAs();
			}
		}

		public void FileSaveAs()
		{
			SaveFileDialog dialog = new();
			bool? result = dialog.ShowDialog();
			if (result == true && SpawnsetFileUtils.TryWriteSpawnsetToFile(Spawnset, dialog.FileName))
				UpdateSpawnsetState(Path.GetFileName(dialog.FileName), dialog.FileName);
		}

		public void SurvivalModReplace()
		{
			ConfirmWindow confirmWindow = new("Replace 'survival' mod file", "Are you sure you want to replace the current 'survival' mod file with this spawnset?", false);
			confirmWindow.ShowDialog();
			if (confirmWindow.IsConfirmed && SpawnsetFileUtils.TryWriteSpawnsetToFile(Spawnset, UserHandler.Instance.Settings.SurvivalFileLocation))
				App.Instance.ShowMessage("Success", "Successfully replaced 'survival' mod file with this spawnset.");
		}

		public void SurvivalModDelete()
		{
			ConfirmWindow confirmWindow = new("Delete 'survival' mod file", "Are you sure you want to delete the current 'survival' mod file? This means the original Devil Daggers V3 spawnset will be re-enabled.", false);
			confirmWindow.ShowDialog();
			if (confirmWindow.IsConfirmed)
			{
				File.Delete(UserHandler.Instance.Settings.SurvivalFileLocation);
				App.Instance.ShowMessage("Success", "Successfully deleted 'survival' mod file.");
			}
		}
	}
}
