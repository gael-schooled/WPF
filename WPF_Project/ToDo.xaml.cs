using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_Project
{
    /// <summary>
    /// Logique d'interaction pour ToDo.xaml
    /// </summary>
    public partial class ToDo : Window
    {
        public ObservableCollection<TaskItem> Tasks { get; set; }

        public ToDo()
        {
            InitializeComponent();
            DataContext = this;
            Tasks = new ObservableCollection<TaskItem>();

            // Tâches d'exemple
            Tasks.Add(new TaskItem { Title = "Analyser les besoins", IsDone = true });
            Tasks.Add(new TaskItem { Title = "Concevoir l'interface", IsDone = false });
            Tasks.Add(new TaskItem { Title = "Développer les fonctionnalités", IsDone = false });
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewTaskTextBox.Text))
            {
                Tasks.Add(new TaskItem { Title = NewTaskTextBox.Text.Trim(), IsDone = false });
                NewTaskTextBox.Clear();
                NewTaskTextBox.Focus();
            }
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksListBox.SelectedItem is TaskItem selectedTask)
            {
                Tasks.Remove(selectedTask);
            }
        }

        private void NewTaskTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                AddTaskButton_Click(sender, e);
            }
        }
    }
}
