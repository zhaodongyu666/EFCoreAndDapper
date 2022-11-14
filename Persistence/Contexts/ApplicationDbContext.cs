using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Persistence.Contexts
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public ApplicationDbContext()
        {
        }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    modelBuilder.Entity<Employee>().ToTable("Employee").HasKey(o => o.Id);
        //    modelBuilder.Entity<Department>().ToTable("Department").HasKey(o => o.Id);

        //    //一个人对应一个考生
        //    modelBuilder.Entity<Employee>()
        //        .HasOne(o => o.Department)
        //        .WithOne().HasForeignKey<Employee>(o => o.DepartmentId);


        //    //modelBuilder.Entity<Employee>()
        //    //    .HasOne(p => p.Department);
        //}


        #region OnModelCreating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Employee>()
                .HasOne(e => e.Department)
                .WithOne(e => e.employee)
                .OnDelete(DeleteBehavior.Cascade);
        }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Scratch;Trusted_Connection=True")
            .UseSqlite("DataSource=test.db");

        //    //if (!_quiet)
        //    //{
        //    //    optionsBuilder.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted });
        //    //}
        //}

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public IDbConnection Connection => Database.GetDbConnection();

    }
}