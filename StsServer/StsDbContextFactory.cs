using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using StsServerIdentity.Data;
using System;

namespace StsServerIdentity
{
    public class StsDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private string _connectionString;

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            if (_connectionString == null)
            {
                _connectionString = "Data Source=.\\SQLEXPRESS;Initial Catalog=StsServer;Integrated Security=True;";
            }
            Console.WriteLine(_connectionString);
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(_connectionString,
                options =>
                {
                    options.EnableRetryOnFailure();
                    options.CommandTimeout(600);
                });
            return new ApplicationDbContext(optionsBuilder.Options);
        }

        public void SetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}
