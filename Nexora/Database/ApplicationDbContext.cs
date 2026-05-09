using Microsoft.EntityFrameworkCore;
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

            userEntity
                .HasOne(x => x.Account)
                .WithOne(x => x.User)
                .HasForeignKey<Account>(x => x.UserId);

            userEntity
                .HasOne(x => x.Session)
                .WithOne(x => x.User)
                .HasForeignKey<Session>(x => x.UserId);

            transactionEntity
                .HasOne(x => x.Sender)
                .WithMany(x => x.SentTransactions)
                .HasForeignKey(x => x.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);

            transactionEntity
                .HasOne(x => x.Receiver)
                .WithMany(x => x.ReceivedTransactions)
                .HasForeignKey(x => x.ReceiverUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
