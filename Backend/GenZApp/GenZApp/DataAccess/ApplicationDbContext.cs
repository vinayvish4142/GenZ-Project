using EntityModel.Models;
using GenZApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SchoolManagement.Infrastructure.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<StudentModel> Students => Set<StudentModel>();

        public DbSet<RefreshToken> RefreshTokens
            => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(x => x.TokenHash).IsUnique();

                entity.Property(x => x.TokenHash)
                    .HasMaxLength(64)
                    .IsRequired();

                entity.HasOne(x => x.User)
                    .WithMany(x => x.RefreshTokens)
                    .HasForeignKey(x => x.UserId);
            });
        }
    }
}