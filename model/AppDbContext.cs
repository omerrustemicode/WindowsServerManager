using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagment.model
{
    public class AppDbContext : DbContext
    {
        public DbSet<Computer> Computers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ComputerManagment;Trusted_Connection=True;");
        }
    }

    public class Computer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double CPUUsage { get; set; }
        public double RAMUsage { get; set; }
        public double DiskUsage { get; set; }
        public DateTime LastUpdated { get; set; }
    }

}
