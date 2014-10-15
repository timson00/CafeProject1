using System;
using System.Data.Entity;

using CafeProject.MobileDataLevel.Entities;

namespace CafeProject.MobileDataLevel.Contexts
{
    public class DatabaseContext : DbContext
    {
        public static DatabaseContext Create()
        {
            return new DatabaseContext();
        }

        public DatabaseContext() : base("DefaultConnection") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Password> Passwords { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<GeneralObject> Objects { get; set; }
        public DbSet<ObjectType> ObjectTypes { get; set; }
        public DbSet<ObjectWorkTime> ObjectWorkTimes { get; set; }
        public DbSet<ObjectLocation> ObjectLocations { get; set; }
        public DbSet<ObjectOption> ObjectOptions { get; set; }
        public DbSet<ObjectComment> ObjectComments { get; set; }
        public DbSet<ObjectStatistic> ObjectStatistics { get; set; }

        // Новые множества
        public DbSet<FoodType> FoodTypes { get; set; } // +
        public DbSet<ObjectMenuItem> ObjectMenuItems { get; set; } // +

        public DbSet<Region> Regions { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Street> Streets { get; set; }
        public DbSet<ObjectAddress> Addresses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
                .HasMany(t => t.Roles)
                .WithMany(t => t.Users)
                .Map(t => t
                    .MapLeftKey("UserID")
                    .MapRightKey("OptionID")
                    .ToTable("UsersInRoles"));

            modelBuilder
                .Entity<GeneralObject>()
                .HasMany(c => c.Options)
                .WithMany(c => c.Objects)
                .Map(t => t
                    .MapLeftKey("ObjectID")
                    .MapRightKey("OptionID")
                    .ToTable("OptionsInObjects"));

            base.OnModelCreating(modelBuilder);
        }
    }
}