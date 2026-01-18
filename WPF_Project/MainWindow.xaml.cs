using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Window = System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace WPF_Project
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnMail_Click(object sender, RoutedEventArgs e)
        {
            Mail maNouvelleFenetre = new Mail(); maNouvelleFenetre.Show();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            Clock maNouvelleFenetre = new Clock(); maNouvelleFenetre.Show();
        }

        private void btn_Con_Click(object sender, RoutedEventArgs e)
        {
            Window1 window1 = new Window1(); window1.Show();
        }

        private void btn_Click_Todo(object sender, RoutedEventArgs e)
        {
            ToDo toDo = new ToDo(); toDo.Show();
        }
    }
}
