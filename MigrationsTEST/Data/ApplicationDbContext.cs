using Microsoft.EntityFrameworkCore;
using MigrationsTEST.Models;

namespace MigrationsTEST.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 設定種子資料
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "張三",
                    Email = "zhang.san@company.com",
                    Department = "IT部門",
                    HireDate = new DateTime(2023, 1, 15),
                    Salary = 50000M
                },
                new Employee
                {
                    Id = 2,
                    Name = "李四",
                    Email = "li.si@company.com",
                    Department = "人力資源部",
                    HireDate = new DateTime(2023, 3, 20),
                    Salary = 45000M
                },
                new Employee
                {
                    Id = 3,
                    Name = "王五",
                    Email = "wang.wu@company.com",
                    Department = "財務部",
                    HireDate = new DateTime(2023, 6, 1),
                    Salary = 48000M
                }
            );
        }
    }
} 