using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace WPF_Project
{
    public class DoneToTextStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Brush))
            {
                bool isDone = value is bool b && b;
                return isDone ? Brushes.Gray : Brushes.Black;
            }
            if (targetType == typeof(TextDecorationCollection))
            {
                bool isDone = value is bool b && b;
                return isDone ? TextDecorations.Strikethrough : null;
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}