using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Helpers;
using Microsoft.EntityFrameworkCore;

namespace HelpSystem.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<ProductMovement> TransferProducts { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Statement> Statements { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<User>()
                .HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<Profile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .HasOne(u => u.Roles)
                .WithMany(r => r.Users);
            modelBuilder.Entity<Products>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Products>()
                .HasOne(u => u.User)
                .WithMany()
                .HasForeignKey(k => k.UserId)
                .OnDelete(DeleteBehavior.SetNull);




            //modelBuilder.Entity<Products>()
            //    .HasOne(u => u.Provider)
            //    .WithMany(p=>p.Products)
            //    .HasForeignKey(p => p.ProviderId);
            //modelBuilder.Entity<Products>()
            //    .HasOne(u => u.Warehouse)
            //    .WithMany(W => W.Products)
            //    .HasForeignKey(k => k.WarehouseID);



            //Автоинкремент
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Role>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Provider>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Products>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Warehouse>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Products>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Statement>()
                .HasOne(s => s.User)
                .WithMany(u => u.Statement)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Statement>()
                .Property(u => u.ID)
                .ValueGeneratedOnAdd();



            var WarehouseService = Guid.NewGuid();
            modelBuilder.Entity<Warehouse>().HasData(
                new Warehouse
                {
                    Id = WarehouseService,
                    Name = "Склад утилизации",
                    IsFreeZing = false,
                    IsService = true,
                    Products = new List<Products>()
                }
                );


            modelBuilder.Entity<Role>().HasData(
                new Role {Id=1, RoleType = UserRoleType.User },
                new Role {Id=2, RoleType = UserRoleType.Admin }
            );

            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Roles)
                .HasForeignKey(u => u.RoleId);


            var adminUserId = Guid.NewGuid();

            var ProfileId = Guid.NewGuid();
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = adminUserId,
                    Name = "Николай",
                    Login = "TotKtoVseZnaet",
                    Password = HashPassword.HashPassowrds("Admin@"),
                    RoleId = 2

                }
            );

            modelBuilder.Entity<Profile>().HasData(
                new Profile
                {
                    Id = ProfileId,
                    UserId = adminUserId,
                    Email = "nikola10www@mail.ru"

                }
            );

        }

    }
}
