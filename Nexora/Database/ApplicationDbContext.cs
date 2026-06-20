using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nexora.Models;


namespace Nexora.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Session> Sessions { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var userEntity = modelBuilder.Entity<User>()
                .ToTable(name: "users");

            var accountEntity = modelBuilder.Entity<Account>()
                .ToTable(name: "accounts");

            var sessionEntity = modelBuilder.Entity<Session>()
                .ToTable(name: "sessions");

            var transactionEntity = modelBuilder.Entity<Transaction>()
                .ToTable(name: "transactions");

            userEntity.HasKey(x => x.Id);
            accountEntity.HasKey(x => x.Id);
            sessionEntity.HasKey(x => x.UserId);
            transactionEntity.HasKey(x => x.Id);

            SeedUserData(userEntity);
            SeedAccountData(accountEntity);

            userEntity
                .HasOne(x => x.Account)
                .WithOne(x => x.User)
                .HasForeignKey<Account>(x => x.UserId);

            userEntity
                .HasOne(x => x.Session)
                .WithOne(x => x.User)
                .HasForeignKey<Session>(x => x.UserId);

            transactionEntity
                .HasOne(x => x.SenderAccount)
                .WithMany(x => x.SentTransactions)
                .HasForeignKey(x => x.SenderAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            transactionEntity
                .HasOne(x => x.ReceiverAccount)
                .WithMany(x => x.ReceivedTransactions)
                .HasForeignKey(x => x.ReceiverAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            transactionEntity
                .Property(x => x.Amount)
                .HasPrecision(18, 2);

            transactionEntity
                .Property( x => x.CreatedAt)
                .HasDefaultValueSql("now()");

            accountEntity
                .Property(x => x.Balance)
                .HasPrecision(18, 2);

            userEntity
                .HasIndex(x => x.Login)
                .IsUnique();
        }

        private void SeedUserData(EntityTypeBuilder<User> userEntity)
        {
            userEntity.HasData(
                new User
                {
                    Id = 1,
                    Login = "admin",
                    Name = "Admin User",
                    PasswordHash = "password123456" //
                },
                new User
                {
                    Id = 2,
                    Login = "user",
                    Name = "Regular User",
                    PasswordHash = "password"
                }
            );
        }
        
        private void SeedAccountData(EntityTypeBuilder<Account> accountEntity)
        {
            accountEntity.HasData(
                new Account
                {
                    Id = 1,
                    UserId =  1,
                    Balance = 1000,
                },
                new Account
                {
                    Id = 2,
                    UserId = 2,
                    Balance = 2000,
                }
            );
        }
    }
}
