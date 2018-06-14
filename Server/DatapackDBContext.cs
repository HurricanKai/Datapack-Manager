using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public class DatapackDBContext : IdentityDbContext<UserModel, IdentityRole, string>
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
#endif

            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "Datapacks.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);

            optionsBuilder.UseSqlite(connection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DatapackModel>(x =>
            {
                x.HasMany(x2 => x2.Versions)
                .WithOne(x3 => x3.Datapack);

                x.HasOne(x2 => x2.Author)
                .WithMany(x3 => x3.Datapacks)
                .IsRequired();

                x.HasMany(x2 => x2.Comments)
                    .WithOne(x3 => x3.Datapack);

                x.Property(x2 => x2.Id)
                .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<DatapackCommentsModel>(x =>
            {
                x.HasOne(x2 => x2.Author)
                .WithMany(x3 => x3.Comments)
                .IsRequired();

                x.Property(x2 => x2.Id)
                .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<DatapackVersionModel>(x =>
            {
                x.Property(x2 => x2.Id)
                .ValueGeneratedOnAdd();
                x.HasIndex(x2 => x2.Name).IsUnique();
            });

            modelBuilder.Entity<DatapackTagModel>(x =>
            {
                x.Property(x2 => x2.Id)
                .ValueGeneratedOnAdd();

                x.HasOne<DatapackModel>(x2 => x2.Datapack)
                .WithMany(x3 => x3.Tags)
                .IsRequired();
            });

            modelBuilder.Entity<DatapackVoteModel>(x =>
            {
                x.Property(x2 => x2.Id).ValueGeneratedOnAdd();
                x.HasOne(x2 => x2.User)
                .WithMany(x2 => x2.Votes)
                .IsRequired();
                x.HasOne(x2 => x2.Datapack)
                .WithMany(x2 => x2.Votes);
            });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<DatapackModel> Datapacks { get; set; }
        public DbSet<DatapackTagModel> Tags { get; set; }
        public DbSet<DatapackVersionModel> DatapackVersionModel { get; set; }
    }
}
