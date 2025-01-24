using LearnerDuo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LearnerDuo.Data
{
    public class LearnerDuoContext : DbContext
    {
        public LearnerDuoContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.Property(u => u.UserId)
                      .UseIdentityColumn(1, 1);
                entity.HasKey(u => u.UserId);

                entity.Property(u => u.UserName)
                    .HasColumnType("nvarchar")
                    .HasMaxLength(50);

                entity.Property(u => u.Gender)
                    .HasColumnType("nvarchar")
                    .HasMaxLength(10);

                entity.Property(u => u.FirstName)
                    .HasColumnType("nvarchar")
                    .HasMaxLength(20);

                entity.Property(u => u.LastName)
                    .HasColumnType("nvarchar")
                    .HasMaxLength(20);

                entity.Property(u => u.City)
                    .HasColumnType("nvarchar")
                    .HasMaxLength(100);

                entity.Property(u => u.Country)
                    .HasColumnType("nvarchar")
                    .HasMaxLength(100);

                entity.Property(u => u.LookingFor)
                    .HasColumnType("nvarchar")
                    .HasMaxLength(500);

                entity.Property(u => u.Interests)
                    .HasColumnType("nvarchar")
                    .HasMaxLength(500);

                entity.Property(u => u.KnownAs)
                    .HasColumnType("nvarchar")
                    .HasMaxLength(100);


                entity.Property(u => u.Description)
                        .HasColumnType("nvarchar(MAX)");

                entity.Property(u => u.Status)
                        .HasColumnType("nvarchar")
                        .HasMaxLength(20);

                entity.Property(u => u.Coin)
                        .HasColumnType("integer");

                entity.Property(u => u.Price)
                        .HasColumnType("integer");

                entity.Property(u => u.Created)
                        .HasColumnType("datetime");

                entity.Property(u => u.Birthday)
                        .HasColumnType("datetime");

                entity.Property(u => u.LastActive)
                        .HasColumnType("datetime");

            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.HasOne(u => u.User)
                      .WithMany(photo => photo.Photos)
                      .HasForeignKey("UserId")
                      .OnDelete(DeleteBehavior.Cascade)
                      .HasConstraintName("FK_User_Photos");

                entity.Property(p => p.PhotoId)
                      .UseIdentityColumn(1, 1);

                entity.HasKey(p => p.PhotoId);

                entity.Property(p => p.PublicId)
                        .HasColumnType("nvarchar(MAX)");
            });

        }
    }
}
