using DevilDaggersCore.Wpf.Utils;
using DevilDaggersSurvivalEditor.Arena;
using DevilDaggersSurvivalEditor.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace DevilDaggersSurvivalEditor.Gui.Windows
{
	public partial class ArenaPresetWindow : Window
	{
		private readonly IEnumerable<PropertyInfo> _properties;

		public ArenaPresetWindow(string presetName)
		{
			InitializeComponent();

			Title = $"{presetName.ToUserFriendlyString()} arena preset";

			_properties = ArenaPresetHandler.Instance.ActivePreset.GetType().GetProperties().Where(p => p.SetMethod != null);

			foreach (PropertyInfo p in _properties)
			{
				Label label = new Label
				{
					Content = p.Name.ToUserFriendlyString(),
				};

				Control control;
				if (p.PropertyType == typeof(bool))
				{
					control = new CheckBox
					{
						Name = p.Name,
						Tag = p.PropertyType,
					};
				}
				else
				{
					TextBox textBox = new TextBox
					{
						Name = p.Name,
						Text = p.GetValue(ArenaPresetHandler.Instance.ActivePreset)?.ToString(),
						Tag = p.PropertyType,
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
			if (sender is not TextBox textBox)
				return;

			Type? type = textBox.Tag as Type;

			bool isValid;
			if (type == typeof(float))
				isValid = float.TryParse(textBox.Text, out _);
			else if (type == typeof(int))
				isValid = int.TryParse(textBox.Text, out _);
			else
				throw new($"Type {type} not supported in ArenaPreset TextBox.");

			textBox.Background = isValid ? ColorUtils.ThemeColors["Gray2"] : ColorUtils.ThemeColors["ErrorBackground"];
		}

		private void OkButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (PropertyInfo p in _properties)
			{
				foreach (UIElement? child in Options.Children)
				{
					if (child is Grid grid)
					{
						foreach (UIElement? gridChild in grid.Children)
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
										throw new($"Type {t} not supported in ArenaPreset TextBox.");
									}
								}
							}
							else if (gridChild is CheckBox checkBox && checkBox.Name == p.Name)
							{
								p.SetValue(ArenaPresetHandler.Instance.ActivePreset, checkBox.IsChecked);
							}
						}
					}
				}
			}

			DialogResult = true;
		}
	}
}
