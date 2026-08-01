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

            userEntity
                .Property(x => x.Id)
                .HasColumnName("id");

            userEntity
                .Property(x => x.Login)
                .HasColumnName("login");

            userEntity
                .Property(x => x.Name)
                .HasColumnName("name");

            userEntity
                .Property(x => x.PasswordHash)
                .HasColumnName("password_hash");

            accountEntity
                .Property(x => x.Id)
                .HasColumnName("id");

            accountEntity
                .Property(x => x.UserId)
                .HasColumnName("user_id");
            accountEntity
                .Property(x => x.Currency)
                .HasColumnName("currency")
                .IsRequired()
                .HasDefaultValue(Currency.RUB);

            sessionEntity
                .Property(x => x.UserId)
                .HasColumnName("user_id");

            sessionEntity
                .Property(x => x.Token)
                .HasColumnName("token");

            sessionEntity
                .Property(x => x.ExpiresAt)
                .HasColumnName("expires_at");

            transactionEntity
                .Property(x => x.Id)
                .HasColumnName("id");

            transactionEntity
                .Property(x => x.SenderAccountId)
                .HasColumnName("sender_account_id");

            transactionEntity
                .Property(x => x.ReceiverAccountId)
                .HasColumnName("receiver_account_id");

            transactionEntity
                .Property(x => x.Currency)
                .HasColumnName("currency")
                .IsRequired()
                .HasDefaultValue(Currency.RUB);

            userEntity.HasKey(x => x.Id);
            accountEntity.HasKey(x => x.Id);
            sessionEntity.HasKey(x => x.UserId);
            transactionEntity.HasKey(x => x.Id);

            SeedUserData(userEntity);
            SeedAccountData(accountEntity);

            userEntity
                .HasMany(x => x.Accounts)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);

            accountEntity
                .HasIndex(x => new { x.UserId, x.Currency })
                .IsUnique();

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
                .HasPrecision(18, 2)
                .HasColumnName("amount");

            transactionEntity
                .Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("now()");

            accountEntity
                .Property(x => x.Balance)
                .HasPrecision(18, 2)
                .HasColumnName("balance");

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
                    UserId = 1,
                    Balance = 1000,
                    Currency = Currency.RUB,
                },
                new Account
                {
                    Id = 2,
                    UserId = 2,
                    Balance = 2000,
                    Currency = Currency.RUB,
                }
            );
        }
    }
}
