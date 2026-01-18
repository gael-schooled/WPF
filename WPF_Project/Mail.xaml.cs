using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
    /// Logique d'interaction pour Mail.xaml
    /// </summary>
    public partial class Mail : Window
    {
        public Mail()
        {
            InitializeComponent();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string host = SmtpHostTextBox.Text?.Trim();
            string portText = SmtpPortTextBox.Text?.Trim();
            bool useSsl = UseSslCheckBox.IsChecked ?? true;
            string user = SmtpUserTextBox.Text?.Trim();
            string password = SmtpPasswordBox.Password;
            string from = FromTextBox.Text?.Trim();
            string to = ToTextBox.Text?.Trim();
            string subject = SubjectTextBox.Text ?? string.Empty;
            string body = BodyTextBox.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(host))
            {
                MessageBox.Show("Veuillez renseigner le serveur SMTP (Host).", "Paramètre manquant", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(portText, out int port))
            {
                MessageBox.Show("Port SMTP invalide.", "Paramètre invalide", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(to))
            {
                MessageBox.Show("Veuillez renseigner au moins un destinataire.", "Destinataire manquant", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(from))
            {
                // Si From non renseigné, essayer de réutiliser user ou avertir
                if (!string.IsNullOrWhiteSpace(user))
                {
                    from = user;
                }
                else
                {
                    MessageBox.Show("Veuillez renseigner l'expéditeur (From) ou le nom d'utilisateur SMTP.", "Expéditeur manquant", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            SendButton.IsEnabled = false;
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(from);
                    // Accepte plusieurs destinataires séparés par ';' ou ','
                    var recipients = to.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim());
                    foreach (var r in recipients)
                    {
                        message.To.Add(r);
                    }

                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = false;

                    using (var client = new SmtpClient(host, port))
                    {
                        client.EnableSsl = useSsl;

                        if (!string.IsNullOrWhiteSpace(user))
                        {
                            client.Credentials = new NetworkCredential(user, password);
                        }
                        else
                        {
                            // Utiliser les credentials par défaut si aucun utilisateur n'est fourni
                            client.UseDefaultCredentials = true;
                        }

                        // SendMailAsync est disponible sous .NET Framework 4.8
                        await client.SendMailAsync(message).ConfigureAwait(false);
                    }
                }

                // Retour au thread UI pour l'alerte
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("E-mail envoyé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Échec de l'envoi : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    SendButton.IsEnabled = true;
                    Mouse.OverrideCursor = null;
                });
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
