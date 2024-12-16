using ComputerManagment.model;
using MahApps.Metro.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ComputerManagment
{
    public partial class MainWindow : Window
    {
        private const string CredentialsFile = "SavedCredentials.json";
        public ObservableCollection<SavedCredential> SavedCredentials { get; set; }
        private readonly DispatcherTimer refreshTimer;

        public MainWindow()
        {
            InitializeComponent();
            SavedCredentials = new ObservableCollection<SavedCredential>();
            ComputerGrid.ItemsSource = SavedCredentials;

            LoadSavedCredentials();

            refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            var credential = new SavedCredential
            {
                IPAddress = IPAddressTextBox.Text,
                Username = UsernameTextBox.Text,
                Password = PasswordBox.Password
            };

            SavedCredentials.Add(credential);
            SaveCredentials();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            foreach (var credential in SavedCredentials)
            {
                MonitorPerformance(credential);
            }
        }

        private void MonitorPerformance(SavedCredential credential)
        {
            Task.Run(() =>
            {
                try
                {
                    string connectionPath = $"\\\\{credential.IPAddress}\\root\\cimv2";
                    var options = new ConnectionOptions
                    {
                        Username = credential.Username,
                        Password = credential.Password,
                        Impersonation = ImpersonationLevel.Impersonate,
                        Authentication = AuthenticationLevel.Packet
                    };

                    var scope = new ManagementScope(connectionPath, options);
                    scope.Connect();

                    credential.CPUUsage = QueryCPUUsage(scope);
                    credential.RAMUsage = QueryRAMUsage(scope);
                    credential.DiskUsage = QueryDiskUsage(scope);

                    Dispatcher.Invoke(() => ComputerGrid.Items.Refresh());
                }
                catch
                {
                    // Log or handle connection failures.
                }
            });
        }

        private string QueryCPUUsage(ManagementScope scope)
        {
            var query = new ObjectQuery("SELECT LoadPercentage FROM Win32_Processor");
            var searcher = new ManagementObjectSearcher(scope, query);

            foreach (var obj in searcher.Get())
            {
                return $"{obj["LoadPercentage"]}%";
            }
            return "N/A";
        }

        private string QueryRAMUsage(ManagementScope scope)
        {
            var query = new ObjectQuery("SELECT FreePhysicalMemory, TotalVisibleMemorySize FROM Win32_OperatingSystem");
            var searcher = new ManagementObjectSearcher(scope, query);

            foreach (var obj in searcher.Get())
            {
                double free = Convert.ToDouble(obj["FreePhysicalMemory"]);
                double total = Convert.ToDouble(obj["TotalVisibleMemorySize"]);
                return $"{Math.Round((1 - free / total) * 100, 2)}%";
            }
            return "N/A";
        }

        private string QueryDiskUsage(ManagementScope scope)
        {
            var query = new ObjectQuery("SELECT FreeSpace, Size FROM Win32_LogicalDisk WHERE DriveType=3");
            var searcher = new ManagementObjectSearcher(scope, query);

            foreach (var obj in searcher.Get())
            {
                double free = Convert.ToDouble(obj["FreeSpace"]);
                double total = Convert.ToDouble(obj["Size"]);
                return $"{Math.Round((1 - free / total) * 100, 2)}%";
            }
            return "N/A";
        }

        private void LoadSavedCredentials()
        {
            if (File.Exists(CredentialsFile))
            {
                string jsonData = File.ReadAllText(CredentialsFile);
                var credentials = JsonSerializer.Deserialize<ObservableCollection<SavedCredential>>(jsonData);

                if (credentials != null)
                {
                    foreach (var credential in credentials)
                    {
                        SavedCredentials.Add(credential);
                    }
                }
            }
        }
        // Open RDP session when the Connect button is clicked
        private void ConnectToRemoteDesktop(object sender, RoutedEventArgs e)
        {
            // Retrieve the saved credential from the button's Tag
            if (sender is Button button && button.Tag is SavedCredential savedCredential)
            {
                string ipAddress = savedCredential.IPAddress;
                string username = savedCredential.Username;
                string password = savedCredential.Password;

                // Launch RDP with the saved credentials
                ConnectToRdp(ipAddress, username, password);
            }
        }

        private void ConnectToRdp(string ipAddress, string username, string password)
        {
            try
            {
                // Build the RDP command
                string rdpCommand = $"/v:{ipAddress}";

                // Start the mstsc.exe process to initiate the remote desktop connection
                Process.Start("mstsc", rdpCommand);

                // Optionally, you can use a third-party RDP library to automatically handle credentials
                // if you want to pass the username and password directly, but Windows RDP doesn't support passing the password directly
                //MessageBox.Show($"RDP session started for {username}@{ipAddress}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start RDP session: {ex.Message}");
            }
        }
        private void SaveCredentials()
        {
            var jsonData = JsonSerializer.Serialize(SavedCredentials, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(CredentialsFile, jsonData);
        }
    }
}
