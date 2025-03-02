using LearnerDuo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LearnerDuo.Data
{
    public class LearnerDuoContext : IdentityDbContext<User, Role, int,
                IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
                IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public LearnerDuoContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Photo> Photos { get; set; }

        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.Property(u => u.Id)
                      .UseIdentityColumn(1, 1);
                entity.HasKey(u => u.Id);

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

                entity.HasMany(ur => ur.UserRoles)
                      .WithOne(u => u.User)
                      .HasForeignKey(ur => ur.UserId)
                      .IsRequired()
                      .HasConstraintName("FK_User_UserRole");

            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasMany(ur => ur.UserRoles)
                      .WithOne(u => u.Role)
                      .HasForeignKey(ur => ur.RoleId)
                      .IsRequired()
                      .HasConstraintName("FK_Role_UserRole");
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

            modelBuilder.Entity<UserLike>()
                        .HasKey(k => new { k.SourceUserId, k.LikedUserId });

            modelBuilder.Entity<UserLike>()
                        .HasOne(s => s.SourceUser)
                        .WithMany(l => l.LikedUsers)
                        .HasForeignKey(s => s.SourceUserId)
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserLike>()
                        .HasOne(s => s.LikedUser)
                        .WithMany(l => l.LikedByUsers)
                        .HasForeignKey(s => s.LikedUserId)
                        .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                        .HasOne(s => s.Sender)
                        .WithMany(m => m.MessagesSent)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                        .HasOne(s => s.Recipient)
                        .WithMany(m => m.MessagesReceived)
                        .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
