namespace ComputerManagment.model
{
    public class SavedCredential
    {
        public string IPAddress { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string CPUUsage { get; set; } = "N/A";
        public string RAMUsage { get; set; } = "N/A";
        public string DiskUsage { get; set; } = "N/A";
    }
}
