using API.Models;
using API.Utilities.Enums;
using API.Utilities.Handler;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class SMDbContext : DbContext
    {
        public SMDbContext(DbContextOptions<SMDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Vendor> Vendors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>().HasIndex(e => e.Email).IsUnique();
            modelBuilder.Entity<Company>().HasIndex(e => e.PhoneNumber).IsUnique();

            modelBuilder.Entity<Company>()
                .HasOne(v => v.Vendor)
                .WithOne(c => c.Company)
                .HasForeignKey<Vendor>(v => v.Guid)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
                .HasOne(a => a.Account)
                .WithOne(c => c.Company)
                .HasForeignKey<Account>(a => a.Guid)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>()
            .HasMany(ar => ar.Accounts)
            .WithOne(e => e.Role)
            .HasForeignKey(ar => ar.RoleGuid)
            .OnDelete(DeleteBehavior.Restrict);

            SeedAccountAndAccount(modelBuilder);
            SeedRole(modelBuilder);
        }

        private void SeedRole(ModelBuilder modelBuilder)
        {
            var roleId = Guid.NewGuid();

            var role = new Role
            {
                Guid = roleId,
                Name = "vendor",
            };

            modelBuilder.Entity<Role>().HasData(role);
        }

        private void SeedAccountAndAccount(ModelBuilder modelBuilder)
        {
            var roleId1 = Guid.NewGuid();
            var roleId2 = Guid.NewGuid();
            var accountId1 = Guid.NewGuid();
            var accountId2 = Guid.NewGuid();

            var role1 = new Role
            {
                Guid = roleId1,
                Name = "admin",

            };

            var role2 = new Role
            {
                Guid = roleId2,
                Name = "manager",
            };

            var account1 = new Account
            {
                Guid = accountId1,
                Password = HashHandler.HashPassword("Admin2023"),
                RoleGuid = roleId1,
                Status = StatusAccount.Approved,
            };

            var account2 = new Account
            {
                Guid = accountId2,
                Password = HashHandler.HashPassword("Manager2023"),
                RoleGuid = roleId2,
                Status = StatusAccount.Approved,
            };

            var company = new Company
            {
                Guid = accountId1,
                Name = "Admin",
                Email = "admin2023@mail.com",
                Address = "null",
                PhoneNumber = "00000",
                Foto = "null",
                CreatedDate = DateTime.Now,
            };

            var company2 = new Company
            {
                Guid = accountId2,
                Name = "Manager Logistic",
                Email = "manager123@mail.com",
                Address = "null",
                PhoneNumber = "11111",
                Foto = "nul",
                CreatedDate = DateTime.Now,
            };

            modelBuilder.Entity<Role>().HasData(role1);
            modelBuilder.Entity<Account>().HasData(account1);
            modelBuilder.Entity<Company>().HasData(company);

            modelBuilder.Entity<Role>().HasData(role2);
            modelBuilder.Entity<Account>().HasData(account2);
            modelBuilder.Entity<Company>().HasData(company2);
        }

    }
}
