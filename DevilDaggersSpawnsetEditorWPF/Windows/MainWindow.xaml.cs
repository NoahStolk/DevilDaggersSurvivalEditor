using DevilDaggersSpawnsetEditorWPF.Helpers;
using DevilDaggersSpawnsetEditorWPF.Models;
using DevilDaggersSpawnsetEditorWPF.Presets;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DevilDaggersSpawnsetEditorWPF.Windows
{
	public partial class MainWindow : Window
	{
		private Spawnset spawnset;

		public MainWindow()
		{
			InitializeComponent();

			CreateEmptySpawnset();

			// Add height map
			TextBlock textBlock = new TextBlock { Background = new SolidColorBrush(GetColorFromHeight(-1)) };
			Grid.SetRow(textBlock, 0);
			Grid.SetColumn(textBlock, 0);
			HeightMap.Children.Add(textBlock);

			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 16; j++)
				{
					textBlock = new TextBlock { Background = new SolidColorBrush(GetColorFromHeight(i * 16 + j)) };

					Grid.SetRow(textBlock, i + 1);
					Grid.SetColumn(textBlock, j);
					HeightMap.Children.Add(textBlock);
				}
			}
		}

		private void CreateEmptySpawnset()
		{
			spawnset = new Spawnset();

			UpdateSpawnsGUI();

			UpdateSettingsGUI();

			UpdateArenaGUI();
		}

		/// <summary>
		/// Updates the internal end loop.
		/// Only call this when the spawns in the spawnset have been modified.
		/// </summary>
		private void UpdateEndLoop()
		{
			for (int i = 0; i < spawnset.spawns.Count; i++)
			{
				spawnset.spawns[i].loop = true;
				if (spawnset.spawns[i].enemy == GameHelper.enemies[-1])
				{
					for (int j = 0; j < i; j++)
					{
						spawnset.spawns[j].loop = false;
					}
				}
			}
		}

		private void UpdateSpawnsGUI()
		{
			// TODO: Optimise (don't regenerate all every time)

			ListBoxSpawns.Items.Clear();

			double seconds = 0;
			int totalGems = 0;
			foreach (KeyValuePair<int, Spawn> kvp in spawnset.spawns)
			{
				seconds += kvp.Value.delay;
				totalGems += kvp.Value.enemy.gems;

				Grid grid = new Grid
				{
					Width = 372,
					HorizontalAlignment = HorizontalAlignment.Left,
					Margin = new Thickness(0)
				};
				for (int i = 0; i < 6; i++)
					grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength((i == 0) ? 1 : 2, GridUnitType.Star) });

				Label l1 = new Label { Padding = new Thickness(4,0,0,0), Content = kvp.Key };
				Label l2 = new Label { Padding = new Thickness(4,0,0,0), Content = seconds.ToString("0.0000") };
				Label l3 = new Label { Padding = new Thickness(4,0,0,0), Content = kvp.Value.enemy.name, FontWeight = (kvp.Value.loop ? FontWeights.Bold : FontWeights.Normal) };
				Label l4 = new Label { Padding = new Thickness(4,0,0,0), Content = kvp.Value.delay.ToString("0.0000") };
				Label l5 = new Label { Padding = new Thickness(4,0,0,0), Content = kvp.Value.enemy.gems };
				Label l6 = new Label { Padding = new Thickness(4,0,0,0), Content = totalGems };

				Grid.SetColumn(l1, 0);
				Grid.SetColumn(l2, 1);
				Grid.SetColumn(l3, 2);
				Grid.SetColumn(l4, 3);
				Grid.SetColumn(l5, 4);
				Grid.SetColumn(l6, 5);

				grid.Children.Add(l1);
				grid.Children.Add(l2);
				grid.Children.Add(l3);
				grid.Children.Add(l4);
				grid.Children.Add(l5);
				grid.Children.Add(l6);

				ListBoxSpawns.Items.Add(grid);
			}
		}

		private void UpdateSettingsGUI()
		{
			TextBlockShrinkStart.Text = spawnset.shrinkStart.ToString();
			TextBlockShrinkEnd.Text = spawnset.shrinkEnd.ToString();
			TextBlockShrinkRate.Text = spawnset.shrinkRate.ToString();
			TextBlockBrightness.Text = spawnset.brightness.ToString();

			EllipseGeometry ellipseShrinkStart = (EllipseGeometry)ShrinkStart.Data;
			ellipseShrinkStart.RadiusX = spawnset.shrinkStart * 2;
			ellipseShrinkStart.RadiusY = spawnset.shrinkStart * 2;

			EllipseGeometry ellipseShrinkEnd = (EllipseGeometry)ShrinkEnd.Data;
			ellipseShrinkEnd.RadiusX = spawnset.shrinkEnd * 2;
			ellipseShrinkEnd.RadiusY = spawnset.shrinkEnd * 2;

			if (spawnset.shrinkRate > 0)
				ShrinkCurrentSlider.Maximum = (spawnset.shrinkStart - spawnset.shrinkEnd) / spawnset.shrinkRate;
			else
				ShrinkCurrentSlider.Maximum = 1;

			EllipseGeometry ellipseShrinkCurrent = (EllipseGeometry)ShrinkCurrent.Data;
			ellipseShrinkCurrent.RadiusX = spawnset.shrinkStart * 2 - (ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (spawnset.shrinkStart - spawnset.shrinkEnd) * 2);
			ellipseShrinkCurrent.RadiusY = spawnset.shrinkStart * 2 - (ShrinkCurrentSlider.Value / ShrinkCurrentSlider.Maximum * (spawnset.shrinkStart - spawnset.shrinkEnd) * 2);
		}

		private Color GetColorFromHeight(float height)
		{
			if (height < Settings.TILE_MIN)
				return Color.FromRgb(0, 0, 0);

			float colorVal = Math.Max(0, (float)Math.Round((height - Settings.TILE_MIN) * 12 + 32));

			return Color.FromRgb((byte)(colorVal), (byte)(colorVal / 2), (byte)(Math.Floor((height - Settings.TILE_MIN) / 16) * 64));
		}

		private void UpdateArenaGUI()
		{
			ArenaTiles.Children.Clear();

			for (int i = 0; i < spawnset.arenaTiles.GetLength(0); i++)
			{
				for (int j = 0; j < spawnset.arenaTiles.GetLength(1); j++)
				{
					float height = spawnset.arenaTiles[i, j];

					Rectangle rect = new Rectangle
					{
						Width = 8,
						Height = 8
					};
					Canvas.SetLeft(rect, i * 8);
					Canvas.SetTop(rect, j * 8);
					SetTileColor(rect);

					ArenaTiles.Children.Add(rect);
				}
			}
		}

		private void SetTileColor(Rectangle rect)
		{
			Point arenaCenter = new Point(204, 204);

			int i = (int)Canvas.GetLeft(rect) / 8;
			int j = (int)Canvas.GetTop(rect) / 8;
			float height = spawnset.arenaTiles[i, j];
			if (height < Settings.TILE_MIN)
				return;

			int x, y;
			if (i > 25)
				x = i * 8 + 7;
			else
				x = i * 8 + 1;

			if (j > 25)
				y = j * 8 + 7;
			else
				y = j * 8 + 1;

			double distance = Math.Sqrt(Math.Pow(x - arenaCenter.X, 2) + Math.Pow(y - arenaCenter.Y, 2)) / 8;

			EllipseGeometry e = (EllipseGeometry)ShrinkCurrent.Data;

			SolidColorBrush color = new SolidColorBrush(GetColorFromHeight(height));
			if (Math.Abs(distance) <= (e.RadiusX + 4) / 8)
				rect.Fill = color;
			else
				rect.Fill = new SolidColorBrush(Color.FromArgb(color.Color.A, (byte)(color.Color.R), (byte)(color.Color.G / 4), (byte)(color.Color.B / 4)));
		}

		private void AddSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			if (!double.TryParse(TextBoxDelay.Text, out double delay))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid delay value");
				return;
			}

			spawnset.spawns.Add(spawnset.spawns.Count, new Spawn(GameHelper.enemies[ComboBoxEnemy.SelectedIndex - 1], delay, true));

			UpdateEndLoop();

			UpdateSpawnsGUI();
		}

		private void InsertSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			if (!double.TryParse(TextBoxDelay.Text, out double delay))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid delay value");
				return;
			}

			int index = ListBoxSpawns.SelectedIndex;
			if (index == -1)
				return; // nothing selected

			List<Spawn> shift = new List<Spawn>();
			int originalCount = spawnset.spawns.Count;
			for (int i = index; i < originalCount; i++)
			{
				shift.Add(spawnset.spawns[i]);
				spawnset.spawns.Remove(i);
			}

			spawnset.spawns.Add(index, new Spawn(GameHelper.enemies[ComboBoxEnemy.SelectedIndex - 1], delay, true));

			int max = spawnset.spawns.Count;
			for (int i = 0; i < shift.Count; i++)
				spawnset.spawns.Add(max + i, shift[i]);

			UpdateEndLoop();

			UpdateSpawnsGUI();
		}

		private void EditSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			if (!double.TryParse(TextBoxDelay.Text, out double delay))
			{
				MessageBox.Show("Please enter a numeric value.", "Invalid delay value");
				return;
			}

			List<int> indices = (from object obj in ListBoxSpawns.SelectedItems
								 select ListBoxSpawns.Items.IndexOf(obj)).ToList();

			foreach (int i in indices)
			{
				spawnset.spawns[i].enemy = GameHelper.enemies[ComboBoxEnemy.SelectedIndex - 1];
				spawnset.spawns[i].delay = delay;
			}

			UpdateEndLoop();

			UpdateSpawnsGUI();
		}

		private void DeleteSpawnButton_Click(object sender, RoutedEventArgs e)
		{
			List<int> indices = (from object obj in ListBoxSpawns.SelectedItems
								 select ListBoxSpawns.Items.IndexOf(obj)).ToList();

			foreach (int i in indices)
			{
				spawnset.spawns.Remove(i);
			}

			// Reset the keys (we don't want gaps in the sorted dictionary)
			SortedDictionary<int, Spawn> newSpawns = new SortedDictionary<int, Spawn>();
			int j = 0;
			foreach (KeyValuePair<int, Spawn> kvp in spawnset.spawns)
			{
				newSpawns.Add(j, kvp.Value);
				j++;
			}

			spawnset.spawns = newSpawns;

			UpdateEndLoop();

			UpdateSpawnsGUI();
		}

		private void FileNew_Click(object sender, RoutedEventArgs e)
		{
			// TODO: Prompt

			CreateEmptySpawnset();
		}

		private void FileOpen_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			bool? result = dialog.ShowDialog();

			if (result.HasValue && result.Value)
			{
				if (!Spawnset.TryParse(dialog.FileName, out spawnset))
				{
					MessageBox.Show("Please open a valid Devil Daggers V3 spawnset file.", "Could not parse file");
				}
			}

			UpdateSpawnsGUI();

			UpdateSettingsGUI();

			UpdateArenaGUI();
		}

		private void FileSave_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			bool? result = dialog.ShowDialog();

			if (result.HasValue && result.Value)
			{
				File.WriteAllBytes(dialog.FileName, spawnset.GetBytes());
			}
		}

		private void SettingsEditApplyButton_Click(object sender, RoutedEventArgs e)
		{
			Button b = (Button)sender;
			if ((string)b.Content == "Apply")
			{
				if (!float.TryParse(TextBoxShrinkStart.Text, out float shrinkStart))
				{
					MessageBox.Show("Please enter a numeric value.", "Invalid shrink start value");
					return;
				}
				if (!float.TryParse(TextBoxShrinkEnd.Text, out float shrinkEnd))
				{
					MessageBox.Show("Please enter a numeric value.", "Invalid shrink end value");
					return;
				}
				if (!float.TryParse(TextBoxShrinkRate.Text, out float shrinkRate))
				{
					MessageBox.Show("Please enter a numeric value.", "Invalid shrink rate value");
					return;
				}
				if (!float.TryParse(TextBoxBrightness.Text, out float brightness))
				{
					MessageBox.Show("Please enter a numeric value.", "Invalid brightness value");
					return;
				}

				spawnset.shrinkStart = shrinkStart;
				spawnset.shrinkEnd = shrinkEnd;
				spawnset.shrinkRate = shrinkRate;
				spawnset.brightness = brightness;

				UpdateSettingsGUI();

				TextBlockShrinkStart.Visibility = Visibility.Visible;
				TextBlockShrinkEnd.Visibility = Visibility.Visible;
				TextBlockShrinkRate.Visibility = Visibility.Visible;
				TextBlockBrightness.Visibility = Visibility.Visible;

				TextBoxShrinkStart.Visibility = Visibility.Collapsed;
				TextBoxShrinkEnd.Visibility = Visibility.Collapsed;
				TextBoxShrinkRate.Visibility = Visibility.Collapsed;
				TextBoxBrightness.Visibility = Visibility.Collapsed;

				b.Content = "Edit";
			}
			else if ((string)b.Content == "Edit")
			{
				TextBoxShrinkStart.Visibility = Visibility.Visible;
				TextBoxShrinkEnd.Visibility = Visibility.Visible;
				TextBoxShrinkRate.Visibility = Visibility.Visible;
				TextBoxBrightness.Visibility = Visibility.Visible;

				TextBoxShrinkStart.Text = TextBlockShrinkStart.Text;
				TextBoxShrinkEnd.Text = TextBlockShrinkEnd.Text;
				TextBoxShrinkRate.Text = TextBlockShrinkRate.Text;
				TextBoxBrightness.Text = TextBlockBrightness.Text;

				TextBlockShrinkStart.Visibility = Visibility.Collapsed;
				TextBlockShrinkEnd.Visibility = Visibility.Collapsed;
				TextBlockShrinkRate.Visibility = Visibility.Collapsed;
				TextBlockBrightness.Visibility = Visibility.Collapsed;

				b.Content = "Apply";
			}
		}

		private void ArenaTiles_MouseMove(object sender, MouseEventArgs e)
		{
			Point tile = Mouse.GetPosition((IInputElement)sender);
			tile = new Point((int)Math.Min(Settings.ARENA_WIDTH - 1, tile.X / 8), (int)Math.Min(Settings.ARENA_HEIGHT - 1, tile.Y / 8));

			float height = spawnset.arenaTiles[(int)tile.Y, (int)tile.X];

			LabelTile.Content = tile.ToString();
			HeightTile.Content = height.ToString();
		}

		private void ArenaTiles_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			Point tile = Mouse.GetPosition((IInputElement)sender);
			tile = new Point((int)tile.X / 8, (int)tile.Y / 8);

			spawnset.arenaTiles[(int)tile.Y, (int)tile.X] = Math.Max(Math.Min(spawnset.arenaTiles[(int)tile.Y, (int)tile.X] + e.Delta / 120, Settings.TILE_MAX), Settings.TILE_MIN);

			Rectangle rect = (Rectangle)ArenaTiles.Children.Cast<UIElement>().First(ee => Canvas.GetTop(ee) == tile.Y * 8 && Canvas.GetLeft(ee) == tile.X * 8);
			rect.Fill = new SolidColorBrush(GetColorFromHeight(spawnset.arenaTiles[(int)tile.Y, (int)tile.X]));
		}

		private void ArenaTiles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Point tile = Mouse.GetPosition((IInputElement)sender);
			tile = new Point((int)tile.X / 8, (int)tile.Y / 8);

			if (spawnset.arenaTiles[(int)tile.Y, (int)tile.X] >= Settings.TILE_MIN)
				spawnset.arenaTiles[(int)tile.Y, (int)tile.X] = Settings.TILE_VOID_DEFAULT;
			else
				spawnset.arenaTiles[(int)tile.Y, (int)tile.X] = Settings.TILE_DEFAULT;

			Rectangle rect = (Rectangle)ArenaTiles.Children.Cast<UIElement>().First(ee => Canvas.GetTop(ee) == tile.Y * 8 && Canvas.GetLeft(ee) == tile.X * 8);
			rect.Fill = new SolidColorBrush(GetColorFromHeight(spawnset.arenaTiles[(int)tile.Y, (int)tile.X]));
		}

		private void ButtonArenaGenerate_Click(object sender, RoutedEventArgs e)
		{
			int type = ComboBoxArenaPreset.SelectedIndex;

			byte[] defaultArenaBuffer = new byte[Settings.ARENA_BUFFER_SIZE];
			FileStream fs = new FileStream("Content/V3_Sorath", FileMode.Open, FileAccess.Read)
			{
				Position = Settings.HEADER_BUFFER_SIZE
			};
			fs.Read(defaultArenaBuffer, 0, Settings.ARENA_BUFFER_SIZE);
			fs.Close();

			switch (type)
			{
				case 0:
				case 1:
					for (int i = 0; i < defaultArenaBuffer.Length; i += 4)
					{
						int x = (i) / (Settings.ARENA_WIDTH * 4);
						int y = ((i) / 4) % Settings.ARENA_HEIGHT;
						spawnset.arenaTiles[x, y] = (type == 0) ? BitConverter.ToSingle(defaultArenaBuffer, i) : (float)Math.Round(BitConverter.ToSingle(defaultArenaBuffer, i));
					}
					break;
				case 2:
					for (int i = 0; i < Settings.ARENA_WIDTH; i++)
						for (int j = 0; j < Settings.ARENA_HEIGHT; j++)
							spawnset.arenaTiles[i, j] = Settings.TILE_DEFAULT;
					break;
				case 3:
					for (int i = 0; i < Settings.ARENA_WIDTH; i++)
						for (int j = 0; j < Settings.ARENA_HEIGHT; j++)
							spawnset.arenaTiles[i, j] = Settings.TILE_VOID_DEFAULT;
					break;
				case 5:
					WindowArenaRectangular rectangleDialog = new WindowArenaRectangular();
					if (rectangleDialog.ShowDialog() == true)
					{
						ArenaRectangular arena = rectangleDialog.arena;
						for (int i = 0; i < Settings.ARENA_WIDTH; i++)
						{
							for (int j = 0; j < Settings.ARENA_HEIGHT; j++)
							{
								spawnset.arenaTiles[i, j] = Settings.TILE_VOID_DEFAULT;
							}
						}
						
						for (int i = arena.x1; i < arena.x2; i++)
							for (int j = arena.y1; j < arena.y2; j++)
								spawnset.arenaTiles[i, j] = arena.height;
					}
					break;
				case 6:
					for (int i = 0; i < Settings.ARENA_WIDTH; i++)
						for (int j = 0; j < Settings.ARENA_HEIGHT; j++)
							spawnset.arenaTiles[i, j] = Settings.TILE_DEFAULT + (25 - Math.Abs(i - 25)) / 3f - 4;
					break;
				case 7:
					for (int i = 0; i < defaultArenaBuffer.Length; i += 4)
					{
						int x = (i) / (Settings.ARENA_WIDTH * 4);
						int y = ((i) / 4) % Settings.ARENA_HEIGHT;
						spawnset.arenaTiles[x, y] = (float)Math.Round(BitConverter.ToSingle(defaultArenaBuffer, i));
					}

					for (int i = 1; i < Settings.ARENA_WIDTH - 1; i++)
						for (int j = 1; j < Settings.ARENA_HEIGHT - 1; j++)
							if ((spawnset.arenaTiles[i - 1, j] < Settings.TILE_DEFAULT
							 || spawnset.arenaTiles[i + 1, j] < Settings.TILE_DEFAULT
							 || spawnset.arenaTiles[i, j - 1] < Settings.TILE_DEFAULT
							 || spawnset.arenaTiles[i, j + 1] < Settings.TILE_DEFAULT)
							 && spawnset.arenaTiles[i, j] == Settings.TILE_DEFAULT)
								spawnset.arenaTiles[i, j] = 16;
					break;
				case 8:
					float a = 0, b = 0;
					for (int i = 0; i < Settings.ARENA_WIDTH; i++)
					{
						a += 0.05f;
						for (int j = 0; j < Settings.ARENA_HEIGHT; j++)
						{
							b += 0.05f;
							spawnset.arenaTiles[i, j] = Utils.r.Next(-1, 1) + a + b - 60;
						}
					}
					break;
			}

			UpdateArenaGUI();
		}

		private void ShrinkCurrentSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			EllipseGeometry ellipseShrinkCurrent = (EllipseGeometry)ShrinkCurrent.Data;
			ellipseShrinkCurrent.RadiusX = (spawnset.shrinkStart - (e.NewValue / ShrinkCurrentSlider.Maximum * (spawnset.shrinkStart - spawnset.shrinkEnd))) * 2;
			ellipseShrinkCurrent.RadiusY = (spawnset.shrinkStart - (e.NewValue / ShrinkCurrentSlider.Maximum * (spawnset.shrinkStart - spawnset.shrinkEnd))) * 2;

			for (int i = 0; i < spawnset.arenaTiles.GetLength(0); i++)
			{
				for (int j = 0; j < spawnset.arenaTiles.GetLength(1); j++)
				{
					Rectangle rect = (Rectangle)ArenaTiles.Children.Cast<UIElement>().First(ee => Canvas.GetTop(ee) == j * 8 && Canvas.GetLeft(ee) == i * 8);

					SetTileColor(rect);
				}
			}
		}
	}
}