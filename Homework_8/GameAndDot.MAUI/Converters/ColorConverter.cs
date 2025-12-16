using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Globalization;
using Microsoft.Maui.Graphics;

namespace GameAndDot.MAUI.Converters
{
    // для конвертации строкового имени цвета в объект Color
    public class ColorConverter : IValueConverter
    {
        public static bool TryParseColor(string colorName, out Color color)
        {
            return Color.TryParse(colorName, out color);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorName && TryParseColor(colorName, out Color color))
            {
                return color;
            }
            return Colors.Black; // Цвет по умолчанию
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
