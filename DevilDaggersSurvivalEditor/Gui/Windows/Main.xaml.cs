using DevilDaggersCore.Wpf.Windows;
using DevilDaggersSurvivalEditor.Arena;
using DevilDaggersSurvivalEditor.Core;
using DevilDaggersSurvivalEditor.Spawnsets;
using DevilDaggersSurvivalEditor.User;
using DevilDaggersSurvivalEditor.Utils;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DevilDaggersSurvivalEditor.Gui.Windows;

public partial class MainWindow : Window
{
	public static readonly RoutedUICommand NewCommand = new("New", nameof(NewCommand), typeof(MainWindow), new() { new KeyGesture(Key.N, ModifierKeys.Control) });
	public static readonly RoutedUICommand OpenCommand = new("Open", nameof(OpenCommand), typeof(MainWindow), new() { new KeyGesture(Key.O, ModifierKeys.Control) });
	public static readonly RoutedUICommand OpenWebCommand = new("Open from DevilDaggers.info", nameof(OpenWebCommand), typeof(MainWindow), new() { new KeyGesture(Key.I, ModifierKeys.Control) });
	public static readonly RoutedUICommand OpenDefaultCommand = new("Open default (V3)", nameof(OpenDefaultCommand), typeof(MainWindow), new() { new KeyGesture(Key.D, ModifierKeys.Control | ModifierKeys.Shift) });
	public static readonly RoutedUICommand SaveCommand = new("Save", nameof(SaveCommand), typeof(MainWindow), new() { new KeyGesture(Key.S, ModifierKeys.Control) });
	public static readonly RoutedUICommand SaveAsCommand = new("Save as", nameof(SaveAsCommand), typeof(MainWindow), new() { new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift) });
	public static readonly RoutedUICommand OpenModCommand = new("Open 'survival' file", nameof(OpenModCommand), typeof(MainWindow), new() { new KeyGesture(Key.O, ModifierKeys.Control | ModifierKeys.Shift) });
	public static readonly RoutedUICommand ReplaceCommand = new("Replace 'survival' file", nameof(ReplaceCommand), typeof(MainWindow), new() { new KeyGesture(Key.R, ModifierKeys.Control) });
	public static readonly RoutedUICommand DeleteCommand = new("Delete 'survival' file", nameof(DeleteCommand), typeof(MainWindow), new() { new KeyGesture(Key.D, ModifierKeys.Control) });
	public static readonly RoutedUICommand ExitCommand = new("Exit", nameof(ExitCommand), typeof(MainWindow), new() { new KeyGesture(Key.F4, ModifierKeys.Alt) });

	public MainWindow()
	{
		InitializeComponent();

		App.Instance.MainWindow = this;
		App.Instance.UpdateMainWindowTitle();

		Closed += (_, _) => Application.Current.Shutdown();

		WarningVoidSpawn.Text = $"The tile at coordinate {TileUtils.SpawnTile} (player spawn) is void, meaning the player will die instantly. You can prevent this from happening in the Options > Settings menu.";

		string survivalPath = Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, "mods", "survival");
		while (Directory.Exists(survivalPath))
			App.Instance.ShowError("Survival folder found in mods folder", $"There should not be a folder named 'survival' in the 'mods' folder ({survivalPath}). Make sure this folder does not exist and then try again.");

		UpdateWarningDevilDaggersRootFolder();

		SpawnsetArena.Initialize();

		if (UserHandler.Instance.Settings.LoadSurvivalFileOnStartUp && UserHandler.Instance.Settings.SurvivalFileExists)
		{
			if (!Spawnset.TryParse(File.ReadAllBytes(UserHandler.Instance.Settings.SurvivalFileLocation), out Spawnset spawnset))
			{
				App.Instance.ShowError("Could not parse file", "Failed to parse the 'survival' file.");
				return;
			}

			SpawnsetHandler.Instance.Spawnset = spawnset;

			App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnset();
			App.Instance.MainWindow?.SpawnsetArena.UpdateSpawnset();
			App.Instance.MainWindow?.SpawnsetSettings.UpdateSpawnset();

			SpawnsetHandler.Instance.UpdateSpawnsetState("(survival)", UserHandler.Instance.Settings.SurvivalFileLocation);
		}

#if DEBUG
		MenuItem debugItem = new() { Header = "Open debug window" };
		debugItem.Click += (_, _) =>
		{
			DebugWindow debugWindow = new();
			debugWindow.ShowDialog();
		};

		MenuItem debugHeader = new() { Header = "Debug" };
		debugHeader.Items.Add(debugItem);

		MenuPanel.Items.Add(debugHeader);
#endif
	}

	private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
		=> e.CanExecute = true;

	private void New_Executed(object sender, ExecutedRoutedEventArgs e)
	{
		if (SpawnsetHandler.Instance.ProceedWithUnsavedChanges())
			return;

		SpawnsetHandler.Instance.Spawnset = new()
		{
			ArenaTiles = ArenaPresetHandler.Instance.DefaultPreset.GetTiles(),
		};

		App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnset();
		App.Instance.MainWindow?.SpawnsetArena.UpdateSpawnset();
		App.Instance.MainWindow?.SpawnsetSettings.UpdateSpawnset();

		SpawnsetHandler.Instance.UpdateSpawnsetState("(new spawnset)", string.Empty);
	}

	private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
	{
		if (SpawnsetHandler.Instance.ProceedWithUnsavedChanges())
			return;

		OpenFileDialog dialog = new();
		bool? result = dialog.ShowDialog();

		if (result == true)
		{
			if (!Spawnset.TryParse(File.ReadAllBytes(dialog.FileName), out Spawnset spawnset))
			{
				App.Instance.ShowError("Could not parse file", "Please open a valid Devil Daggers V3 'survival' file.");
				return;
			}

			SpawnsetHandler.Instance.Spawnset = spawnset;

			App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnset();
			App.Instance.MainWindow?.SpawnsetArena.UpdateSpawnset();
			App.Instance.MainWindow?.SpawnsetSettings.UpdateSpawnset();

			SpawnsetHandler.Instance.UpdateSpawnsetState(Path.GetFileName(dialog.FileName), dialog.FileName);
		}
	}

	private void OpenWeb_Executed(object sender, RoutedEventArgs e)
	{
		DownloadSpawnsetWindow window = new();
		window.ShowDialog();
	}

	private void OpenDefault_Executed(object sender, RoutedEventArgs e)
	{
		if (SpawnsetHandler.Instance.ProceedWithUnsavedChanges())
			return;

		using Stream stream = App.Assembly.GetManifestResourceStream("DevilDaggersSurvivalEditor.Content.survival") ?? throw new("Could not retrieve default survival file resource stream.");
		using BinaryReader reader = new(stream);
		if (!Spawnset.TryParse(reader.ReadBytes((int)stream.Length), out Spawnset spawnset))
		{
			App.Instance.ShowError("Could not parse file", "Default internal 'survival' file is invalid.");
			return;
		}

		SpawnsetHandler.Instance.Spawnset = spawnset;

		App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnset();
		App.Instance.MainWindow?.SpawnsetArena.UpdateSpawnset();
		App.Instance.MainWindow?.SpawnsetSettings.UpdateSpawnset();

		SpawnsetHandler.Instance.UpdateSpawnsetState("(new spawnset)", string.Empty);
	}

	private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
		=> SpawnsetHandler.Instance.FileSave();

	private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
		=> SpawnsetHandler.Instance.FileSaveAs();

	private void OpenMod_Executed(object sender, RoutedEventArgs e)
	{
		if (SpawnsetHandler.Instance.ProceedWithUnsavedChanges())
			return;

		if (!UserHandler.Instance.Settings.SurvivalFileExists)
		{
			App.Instance.ShowMessage("'survival' file does not exist", $"Please make sure the 'survival' file at {UserHandler.Instance.Settings.SurvivalFileLocation} exists.");
			return;
		}

		if (!Spawnset.TryParse(File.ReadAllBytes(UserHandler.Instance.Settings.SurvivalFileLocation), out Spawnset spawnset))
		{
			App.Instance.ShowError("Could not parse 'survival' file", $"Failed to parse the 'survival' file at {UserHandler.Instance.Settings.SurvivalFileLocation}.");
			return;
		}

		SpawnsetHandler.Instance.Spawnset = spawnset;

		App.Instance.MainWindow?.SpawnsetSpawns.UpdateSpawnset();
		App.Instance.MainWindow?.SpawnsetArena.UpdateSpawnset();
		App.Instance.MainWindow?.SpawnsetSettings.UpdateSpawnset();

		SpawnsetHandler.Instance.UpdateSpawnsetState("(survival)", UserHandler.Instance.Settings.SurvivalFileLocation);
	}

	private void Replace_Executed(object sender, ExecutedRoutedEventArgs e)
		=> SpawnsetHandler.Instance.SurvivalModReplace();

	private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
		=> SpawnsetHandler.SurvivalModDelete();

	private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
		=> Application.Current.Shutdown();

	private void Settings_Click(object sender, RoutedEventArgs e)
	{
		SettingsWindow settingsWindow = new();
		if (settingsWindow.ShowDialog() == true)
		{
			UserHandler.Instance.SaveSettings();

			if (App.Instance.MainWindow != null)
			{
				Dispatcher.Invoke(() =>
				{
					App.Instance.MainWindow.UpdateWarningDevilDaggersRootFolder();
					App.Instance.MainWindow.SpawnsetArena.UpdateTile(TileUtils.SpawnTile);
					App.Instance.MainWindow.SpawnsetSpawns.EndLoopPreview.Update();
				});
			}
		}
	}

	private void Browse_Click(object sender, RoutedEventArgs e)
		=> ProcessUtils.OpenUrl(UrlUtils.SpawnsetsPage);

	private void Discord_Click(object sender, RoutedEventArgs e)
		=> ProcessUtils.OpenUrl(UrlUtils.DiscordInviteLink);

	private void Help_Click(object sender, RoutedEventArgs e)
		=> ProcessUtils.OpenUrl(UrlUtils.GuidePage);

	private void About_Click(object sender, RoutedEventArgs e)
	{
		AboutWindow aboutWindow = new();
		aboutWindow.ShowDialog();
	}

	private void ViewSourceCode_Click(object sender, RoutedEventArgs e)
		=> ProcessUtils.OpenUrl(UrlUtils.SourceCode);

	public void UpdateWarningDevilDaggersRootFolder()
	{
		bool visible = !File.Exists(Path.Combine(UserHandler.Instance.Settings.DevilDaggersRootFolder, "dd.exe"));
		WarningDevilDaggersRootFolder.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
		WarningDevilDaggersRootFolder.Text = visible ? $"The path {UserHandler.Instance.Settings.DevilDaggersRootFolder} does not seem to be the path where Devil Daggers is installed. Please correct this in the Options > Settings menu." : string.Empty;

		UpdateWarningStackPanel();
	}

	public void UpdateWarningEndLoopLength(bool visible, double loopLength)
	{
		WarningEndLoopLength.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
		WarningEndLoopLength.Text = visible ? $"The end loop is only {loopLength:0.0000} seconds long, which will probably result in Devil Daggers lagging and becoming unstable." : string.Empty;

		UpdateWarningStackPanel();
	}

	public void UpdateWarningVoidSpawn(bool visible)
	{
		WarningVoidSpawn.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;

		UpdateWarningStackPanel();
	}

	private void UpdateWarningStackPanel()
	{
		bool visible =
			WarningDevilDaggersRootFolder.Visibility == Visibility.Visible ||
			WarningEndLoopLength.Visibility == Visibility.Visible ||
			WarningVoidSpawn.Visibility == Visibility.Visible;
		WarningStackPanel.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
	}

	private void Window_Closing(object sender, CancelEventArgs e)
	{
		UserHandler.Instance.SaveSettings();

		e.Cancel = SpawnsetHandler.Instance.ProceedWithUnsavedChanges();
	}
}
