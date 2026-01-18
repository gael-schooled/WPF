using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WPF_Project
{
    public class TaskTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskItem task)
            {
                
                return new
                {
                    Text = task.Title,
                    IsDone = task.IsDone,
                    Foreground = task.IsDone ? Brushes.Gray : Brushes.Black,
                    TextDecoration = task.IsDone ? TextDecorations.Strikethrough : null
                };
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
