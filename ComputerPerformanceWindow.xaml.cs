using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
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
using System.Windows.Threading;

namespace ComputerManagment
{

    public partial class ComputerPerformanceWindow : Window
    {
        private string IpAddress;
        private string Username;
        private string Password;
        private bool isConnected = false; // Flag to track connection status
        private DispatcherTimer refreshTimer;

        public ComputerPerformanceWindow(string ipAddress, string username, string password)
        {
            InitializeComponent();
            IpAddress = ipAddress;
            Username = username;
            Password = password;

            InitializeRefreshTimer();
        }

        private void InitializeRefreshTimer()
        {
            refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10) // Refresh every 10 seconds
            };
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            // Refresh performance only if connected
            if (isConnected)
            {
                ConnectToComputer(IpAddress, Username, Password);
            }
            else
            {
                MessageBox.Show("Connection not established. Please check the credentials.");
            }
        }

        private void ConnectToComputer(string ipAddress, string username, string password)
        {
            try
            {
                // Establish the connection only once
                if (!isConnected)
                {
                    string connectionPath = $"\\\\{ipAddress}\\root\\cimv2";
                    ConnectionOptions options = new ConnectionOptions
                    {
                        Username = username,
                        Password = password,
                        Impersonation = ImpersonationLevel.Impersonate,
                        Authentication = AuthenticationLevel.Packet
                    };

                    ManagementScope scope = new ManagementScope(connectionPath, options);
                    scope.Connect(); // Connect to the remote machine

                    // Mark as connected
                    isConnected = true;

                    MessageBox.Show("Connection successful!");

                    // Now start querying performance stats
                    QueryCPUUsage(scope);
                    QueryRAMUsage(scope);
                    QueryDiskUsage(scope);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("Access denied. Please check your username and password.");
            }
            catch (ManagementException ex)
            {
                MessageBox.Show($"Management error: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to connect: {ex.Message}");
            }
        }

        private void QueryCPUUsage(ManagementScope scope)
        {
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Processor");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            foreach (ManagementObject obj in searcher.Get())
            {
                CPUUsageText.Text = $"{obj["LoadPercentage"]}%";
            }
        }

        private void QueryRAMUsage(ManagementScope scope)
        {
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            foreach (ManagementObject obj in searcher.Get())
            {
                RAMUsageText.Text = $"{Math.Round(Convert.ToDouble(obj["FreePhysicalMemory"]) / Convert.ToDouble(obj["TotalVisibleMemorySize"]) * 100, 2)}%";
            }
        }

        private void QueryDiskUsage(ManagementScope scope)
        {
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_LogicalDisk WHERE DriveType = 3");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

            foreach (ManagementObject obj in searcher.Get())
            {
                double usedSpace = Convert.ToDouble(obj["Size"]) - Convert.ToDouble(obj["FreeSpace"]);
                double diskUsage = usedSpace / Convert.ToDouble(obj["Size"]) * 100;
                DiskUsageText.Text = $"{Math.Round(diskUsage, 2)}%";
            }
        }
    }


}
