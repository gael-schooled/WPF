using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPF_Project
{
    /// <summary>
    /// Logique d'interaction pour Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        // Champs pour UDP
        private UdpClient _udpListener;
        private CancellationTokenSource _udpListenCts;

        // Champs pour TCP Listener (server)
        private TcpListener _tcpListener;
        private CancellationTokenSource _tcpListenerCts;

        // Champs pour TCP Client
        private TcpClient _tcpClient;

        public Window1()
        {
            InitializeComponent();
        }

        private void Envoyer_Click(object sender, RoutedEventArgs e)
        {
            // Bouton Envoyer non lié à un protocole spécifique par la consigne.
            MessageBox.Show("Utilisez les sous-menus Communication -> UDP ou Listener/Client pour envoyer/recevoir.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ---------------------------
        // Listener/Client (TCP)
        // ---------------------------
        private async void ListenerEcouter_Click(object sender, RoutedEventArgs e)
        {
            if (_tcpListener != null)
            {
                MessageBox.Show("Le listener est déjà en cours d'exécution.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                _tcpListener = new TcpListener(IPAddress.Any, 8000);
                _tcpListener.Start();
                _tcpListenerCts = new CancellationTokenSource();

                AppendEchange("Listener démarré sur le port 8000. En attente de connexion...");

                // Accepter une connexion asynchrone
                var client = await _tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);

                // Traiter la connexion sur le thread UI via Dispatcher
                await Dispatcher.InvokeAsync(async () =>
                {
                    AppendEchange("Client connecté.");

                    try
                    {
                        using (var stream = client.GetStream())
                        using (var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true))
                        using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
                        {
                            // Envoyer message de bienvenue au client
                            writer.Write("Connexion réussie");
                            writer.Flush();
                            AppendEchange("Message envoyé au client : Connexion réussie");

                            // Lire le message du client (bloquant jusqu'à réception)
                            string clientMessage = null;
                            try
                            {
                                clientMessage = reader.ReadString();
                            }
                            catch (Exception ex)
                            {
                                AppendEchange("Erreur lecture message client : " + ex.Message);
                            }

                            if (!string.IsNullOrEmpty(clientMessage))
                            {
                                AppendEchange("Message reçu : " + clientMessage);
                            }
                        }
                    }
                    finally
                    {
                        client.Close();
                        AppendEchange("Connexion client fermée.");
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur Listener : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                StopTcpListener();
            }
        }

        private async void ListenerConnecter_Click(object sender, RoutedEventArgs e)
        {
            string serverName = Server.Text;
            if (string.IsNullOrWhiteSpace(serverName))
            {
                MessageBox.Show("Veuillez saisir un nom de serveur.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _tcpClient = new TcpClient();
                AppendEchange($"Tentative de connexion à {serverName}:8000 ...");

                await _tcpClient.ConnectAsync(serverName, 8000).ConfigureAwait(false);

                await Dispatcher.InvokeAsync(() =>
                {
                    AppendEchange("Connecté au serveur.");
                });

                using (var stream = _tcpClient.GetStream())
                using (var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true))
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
                {
                    // Lire le message initial du serveur
                    string serverMessage = null;
                    try
                    {
                        serverMessage = reader.ReadString();
                    }
                    catch (Exception ex)
                    {
                        await Dispatcher.InvokeAsync(() => AppendEchange("Erreur lecture message serveur : " + ex.Message));
                    }

                    if (!string.IsNullOrEmpty(serverMessage))
                    {
                        await Dispatcher.InvokeAsync(() => AppendEchange("Message reçu : " + serverMessage));
                    }

                    // Envoyer le message du client au serveur
                    string machineMessage = $"Machine {Environment.MachineName} connectée";
                    writer.Write(machineMessage);
                    writer.Flush();
                    await Dispatcher.InvokeAsync(() => AppendEchange("Message envoyé : " + machineMessage));
                }

                _tcpClient.Close();
                _tcpClient = null;
                await Dispatcher.InvokeAsync(() => AppendEchange("Connexion fermée."));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur connexion client : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                if (_tcpClient != null)
                {
                    _tcpClient.Close();
                    _tcpClient = null;
                }
            }
        }

        // ---------------------------
        // UDP
        // ---------------------------
        private async void UDPEcoluter_Click(object sender, RoutedEventArgs e)
        {
            if (_udpListener != null)
            {
                MessageBox.Show("Le listener UDP est déjà actif.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                _udpListener = new UdpClient(8080);
                _udpListenCts = new CancellationTokenSource();
                AppendEchange("Listener UDP démarré sur le port 8080.");

                // Écoute en arrière-plan continue jusqu'à annulation
                await Task.Run(async () =>
                {
                    var ct = _udpListenCts.Token;
                    while (!ct.IsCancellationRequested)
                    {
                        try
                        {
                            var result = await _udpListener.ReceiveAsync().ConfigureAwait(false);
                            string msg = Encoding.UTF8.GetString(result.Buffer);

                            await Dispatcher.InvokeAsync(() => AppendEchange("Message reçu : " + msg));
                        }
                        catch (ObjectDisposedException)
                        {
                            // Arrêt demandé
                            break;
                        }
                        catch (Exception ex)
                        {
                            await Dispatcher.InvokeAsync(() => AppendEchange("Erreur UDP réception : " + ex.Message));
                        }
                    }
                }, _udpListenCts.Token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur listener UDP : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                StopUdpListener();
            }
        }

        private async void UDPConnecter_Click(object sender, RoutedEventArgs e)
        {
            string serverName = Server.Text;
            if (string.IsNullOrWhiteSpace(serverName))
            {
                MessageBox.Show("Veuillez saisir un nom de serveur.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string messageToSend = string.IsNullOrEmpty(Message.Text) ? "Message par défaut" : Message.Text;

            try
            {
                // Résolution du nom en IP (IPv4)
                IPAddress[] addresses = Dns.GetHostAddresses(serverName);
                IPAddress ip = null;
                foreach (var a in addresses)
                {
                    if (a.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ip = a;
                        break;
                    }
                }

                if (ip == null)
                {
                    MessageBox.Show("Impossible de résoudre l'adresse IPv4 du serveur.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (var udp = new UdpClient())
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(messageToSend);
                    await udp.SendAsync(bytes, bytes.Length, new IPEndPoint(ip, 8080)).ConfigureAwait(false);
                }

                AppendEchange("Message envoyé : " + messageToSend);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur envoi UDP : " + ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ---------------------------
        // Socket (placeholders)
        // ---------------------------
        private void SocketEcouter_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fonction Socket non implémentée. Utilisez Listener/Client pour TCP.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SocketConnecter_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fonction Socket non implémentée. Utilisez Listener/Client pour TCP.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SocketDeconnecter_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fonction Socket non implémentée.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
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
            else
            {
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

        // ---------------------------
        // Helpers et arrêt/cleanup
        // ---------------------------
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

        private void AppendEchange(string text)
        {
            // Ajoute une ligne à la zone Echanges en s'assurant d'être sur le thread UI
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => AppendEchange(text));
                return;
            }

            if (string.IsNullOrEmpty(Echanges.Text))
                Echanges.Text = text;
            else
                Echanges.Text += Environment.NewLine + text;
        }

        private void StopUdpListener()
        {
            try
            {
                _udpListenCts?.Cancel();
                _udpListener?.Close();
            }
            catch { }
            finally
            {
                _udpListener = null;
                _udpListenCts = null;
                AppendEchange("Listener UDP arrêté.");
            }
        }

        private void StopTcpListener()
        {
            try
            {
                _tcpListenerCts?.Cancel();
                _tcpListener?.Stop();
            }
            catch { }
            finally
            {
                _tcpListener = null;
                _tcpListenerCts = null;
                AppendEchange("Listener TCP arrêté.");
            }
        }

        // Nettoyage à la fermeture de la fenêtre
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            try
            {
                StopUdpListener();
                StopTcpListener();

                if (_tcpClient != null)
                {
                    _tcpClient.Close();
                    _tcpClient = null;
                }
            }
            catch { }
        }
    }
}
