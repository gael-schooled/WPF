using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Envoyer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListenerEcouter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ListenerConnecter_Click(object sender, RoutedEventArgs e)
        {

        }
        private void UDPEcoluter_Click(object sender, RoutedEventArgs e)
        {

        }
        private void UDPConnecter_Click(object sender, RoutedEventArgs e)
        {

        }
        private void SocketEcouter_Click(object sender, RoutedEventArgs e)
        {

        }
        private void SocketConnecter_Click(object sender, RoutedEventArgs e)
        {

        }
        private void SocketDeconnecter_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Verifier_Click(object sender, RoutedEventArgs e)
        {
            String serverName = Server.Text;

            if (string.IsNullOrEmpty(serverName))
            {
                MessageBox.Show("Veuillez saisir un nom de serveur.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isIPv4 = IsValidIPv4(serverName);

            if (!isIPv4)
            {
                MessageBox.Show("L'IP est invalide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else{
                if (PingHost(serverName))
                {
                    IP.Text = serverName;
                    MessageBox.Show($"Vérification réussie !", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    IP.Text = "XX.XX.XX.XX";
                    MessageBox.Show($"L'adresse IP  est valide mais ne répond pas au ping.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool IsValidIPv4(string ip)
        {
            if (string.IsNullOrEmpty(ip))
                return false;

            string pattern = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            return Regex.IsMatch(ip, pattern);
        }

        private bool PingHost(string host)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(host, 3000); 
                    return reply != null && reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

    }
}
