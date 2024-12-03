using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CTBX.EmployeesImport.Shared
{
    public class FilePathDB : DbContext
    {
        private readonly IConfiguration _configuration;

        public FilePathDB(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("FilePathDB");
            optionsBuilder.UseNpgsql(connectionString);
        }

        public DbSet<FileRecord> FileRecords { get; set; } = null!;
    }
}

