using DevilDaggersSurvivalEditor.Code;
using DevilDaggersSurvivalEditor.Code.Arena;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class ArenaPresetWindow : Window
	{
		private readonly IEnumerable<PropertyInfo> properties;

		public ArenaPresetWindow(string presetName)
		{
			InitializeComponent();

			Title = $"{presetName.ToUserFriendlyString()} arena preset";

			properties = ArenaPresetHandler.Instance.ActivePreset.GetType().GetProperties().Where(p => p.SetMethod != null);

			foreach (PropertyInfo p in properties)
			{
				Label label = new Label()
				{
					Content = p.Name.ToUserFriendlyString()
				};

				Control control;
				if (p.PropertyType == typeof(bool))
				{
					control = new CheckBox()
					{
						Name = p.Name,
						Tag = p.PropertyType
					};
				}
				else
				{
					TextBox textBox = new TextBox()
					{
						Name = p.Name,
						Text = p.GetValue(ArenaPresetHandler.Instance.ActivePreset).ToString(),
						Tag = p.PropertyType
					};
					textBox.TextChanged += TextBox_TextChanged;

					control = textBox;
				}

				Grid.SetColumn(control, 1);

				Grid grid = new Grid();
				grid.ColumnDefinitions.Add(new ColumnDefinition());
				grid.ColumnDefinitions.Add(new ColumnDefinition());

				grid.Children.Add(label);
				grid.Children.Add(control);

				Options.Children.Add(grid);
			}
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textBox = sender as TextBox;

			Type t = textBox.Tag as Type;
			bool valid;
			if (t == typeof(float))
			{
				valid = float.TryParse(textBox.Text, out _);
			}
			else if (t == typeof(int))
			{
				valid = int.TryParse(textBox.Text, out _);
			}
			else
			{
				Exception ex = new Exception($"Type {t} not supported in ArenaPreset TextBox.");
				App.Instance.ShowError("Error", $"Type {t} not supported in ArenaPreset TextBox.", ex);
				throw ex;
			}

			textBox.Background = valid ? new SolidColorBrush(Color.FromRgb(255, 255, 255)) : new SolidColorBrush(Color.FromRgb(255, 127, 127));
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (PropertyInfo p in properties)
			{
				foreach (UIElement child in Options.Children)
				{
					if (child is Grid grid)
					{
						foreach (UIElement gridChild in grid.Children)
						{
							if (gridChild is TextBox textBox)
							{
								if (textBox.Name == p.Name)
								{
									Type t = p.PropertyType;

									if (t == typeof(float))
									{
										if (!float.TryParse(textBox.Text, out float value))
											return;
										p.SetValue(ArenaPresetHandler.Instance.ActivePreset, value);
									}
									else if (t == typeof(int))
									{
										if (!int.TryParse(textBox.Text, out int value))
											return;
										p.SetValue(ArenaPresetHandler.Instance.ActivePreset, value);
									}
									else
									{
										Exception ex = new Exception($"Type {t} not supported in ArenaPreset TextBox.");
										App.Instance.ShowError("Error", $"Type {t} not supported in ArenaPreset TextBox.", ex);
										throw ex;
									}
								}
							}
							else if (gridChild is CheckBox checkBox)
							{
								if (checkBox.Name == p.Name)
								{
									p.SetValue(ArenaPresetHandler.Instance.ActivePreset, checkBox.IsChecked);
								}
							}
						}
					}
				}
			}

			DialogResult = true;
		}
	}
}