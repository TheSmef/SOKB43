using API.Security;
using Microsoft.EntityFrameworkCore;
using Models.Entity;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Role>().HasOne(e => e.AccountUser).WithMany(x => x.Roles).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(p => new { p.PassportNumber, p.PassportSeries }).IsUnique();

            modelBuilder.Entity<TypeEquipment>().HasIndex(e => e.Name).IsUnique(true);

            modelBuilder.Entity<Contractor>().HasIndex(e => e.Email).IsUnique(true);
            modelBuilder.Entity<Contractor>().HasIndex(e => e.Name).IsUnique(true);
            modelBuilder.Entity<Contractor>().HasIndex(e => e.PhoneNumber).IsUnique(true);
            modelBuilder.Entity<Order>().HasOne(e => e.Contractor).WithMany(e => e.Orders).OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Equipment>().HasIndex(e => e.EquipmentCode).IsUnique(true);
            modelBuilder.Entity<Equipment>().HasOne(e => e.Order).WithMany(e => e.Equipments).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Equipment>().HasOne(e => e.TechnicalTask).WithMany(e => e.Equipments).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>().HasOne(e => e.Equipment).WithMany(e => e.Services).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TechnicalTest>().HasOne(e => e.Equipment).WithMany(e => e.TechicalTests).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<TechnicalTest>().HasOne(e => e.User).WithMany().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TechnicalTest>().Property(e => e.Comment).IsRequired(false);

            modelBuilder.Entity<TechnicalTask>().HasIndex(e => e.NameEquipment).IsUnique(true);
            modelBuilder.Entity<TechnicalTask>().HasOne(e => e.TypeEquipment).WithMany(e => e.TechnicalTasks).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Token>().HasOne(e => e.Account).WithMany().OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPost>().HasOne(e => e.User).WithMany(e => e.UserPosts).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<UserPost>().HasOne(e => e.Post).WithMany(e => e.UserPosts).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Account>().HasOne(e => e.User).WithOne(x => x.Account).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Token>().HasIndex(e => e.TokenStr).IsUnique(true);


            modelBuilder.Entity<User>().Property(e => e.Otch).IsRequired(false);




            modelBuilder.Entity<Post>().Property(p => p.Salary).HasPrecision(15, 2);
            modelBuilder.Entity<Order>().Property(p => p.Sum).HasPrecision(15, 2);
            modelBuilder.Entity<Service>().Property(p => p.Sum).HasPrecision(15, 2);
            modelBuilder.Entity<UserPost>().Property(p => p.Share).HasPrecision(3, 2);

            modelBuilder.Entity<User>().Property(e => e.Otch).IsRequired(false);
            modelBuilder.Entity<Account>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Account>().HasIndex(u => u.Login).IsUnique();
            modelBuilder.Entity<Account>().ToTable(e => e.HasCheckConstraint("CH_Email_Account", "Email like '%@%.%'"));
            modelBuilder.Entity<Contractor>().ToTable(e => e.HasCheckConstraint("CH_Email_Contractor", "Email like '%@%.%'"));


            modelBuilder.Entity<Post>().ToTable(e => e.HasCheckConstraint("CH_Salary_Post", "Salary > 0"));
            modelBuilder.Entity<Order>().ToTable(e => e.HasCheckConstraint("CH_Sum_Order", "Sum > 0"));
            modelBuilder.Entity<Service>().ToTable(e => e.HasCheckConstraint("CH_Sum_Service", "Sum > 0"));
            modelBuilder.Entity<Post>().HasIndex(u => u.Name).IsUnique();

            modelBuilder.Entity<UserPost>().ToTable(e => e.HasCheckConstraint("Ch_Share_UserPost", "Share > 0 AND Share <= 1"));

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("F0E290A9-9054-4AE7-AF3B-08DAD84FEB5B"),
                    First_name = "Админ",
                    Last_name = "Админ",
                    BirthDate = DateTime.Today.AddYears(-30),
                    PassportSeries = "0000",
                    PassportNumber = "000000",
                    PhoneNumber = "88888888888",
                });
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    UserId = Guid.Parse("F0E290A9-9054-4AE7-AF3B-08DAD84FEB5B"),
                    Email = "admin@admin.com",
                    Login = "admin",
                    Password = HashProvider.MakeHash("admin"),
                });
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = Guid.NewGuid(),
                    AccountUserId = Guid.Parse("F0E290A9-9054-4AE7-AF3B-08DAD84FEB5B"),
                    Name = "Администратор"
                }); 




        }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserPost> UserPosts { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<TechnicalTest> TechnicalTests { get; set; }
        public DbSet<TechnicalTask> TechnicalTasks { get; set; }
        public DbSet<Contractor> Conctractors { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<TypeEquipment> TypesEquipment { get; set; }
    }
}
