using System;
using System.Globalization;
using System.Windows.Data;

namespace DevilDaggersSurvivalEditor.Converters;

public class ComparisonConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		=> value.Equals(parameter);

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		=> value.Equals(true) ? parameter : Binding.DoNothing;
}
