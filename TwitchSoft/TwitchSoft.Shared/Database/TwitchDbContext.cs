using Microsoft.EntityFrameworkCore;
using TwitchSoft.Shared.Database.Models;

namespace TwitchSoft.Shared.Database
{
    public class TwitchDbContext : DbContext
    {
        public TwitchDbContext(DbContextOptions<TwitchDbContext> options) : base(options)
        {
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.EnableSensitiveDataLogging(true);
            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<UserBan> UserBans { get; set; }
        public DbSet<CommunitySubscription> CommunitySubscriptions { get; set; }
        public DbSet<SubscriptionStatistic> SubscriptionStatistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => new { u.JoinChannel });

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .HasMaxLength(60);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Subscription>()
                .HasOne(u => u.User)
                .WithMany(u => u.UserSubscriptions)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Subscription>()
                .HasOne(u => u.Channel)
                .WithMany(u => u.ChannelSubscriptions)
                .HasForeignKey(c => c.ChannelId);

            modelBuilder.Entity<Subscription>()
                .HasOne(u => u.GiftedByUser)
                .WithMany(u => u.UserSubscriptionGifts)
                .HasForeignKey(c => c.GiftedBy);

            modelBuilder.Entity<Subscription>()
                .Property(u => u.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Subscription>()
                .HasIndex(u => new { u.UserId })
                .IncludeProperties(u => new { u.SubscribedTime });

            modelBuilder.Entity<Subscription>()
                .HasIndex(u => new { u.ChannelId })
                .IncludeProperties(u => new { u.SubscribedTime });

            modelBuilder.Entity<UserBan>()
                .Property(u => u.Reason)
                .HasMaxLength(140);

            modelBuilder.Entity<UserBan>()
                .Property(u => u.UserName)
                .HasMaxLength(60);

            modelBuilder.Entity<UserBan>()
                .Property(u => u.Channel)
                .HasMaxLength(60);

            modelBuilder.Entity<UserBan>()
                .HasIndex(u => new { u.UserName });

            modelBuilder.Entity<UserBan>()
                .HasIndex(u => new { u.Channel });

            modelBuilder.Entity<CommunitySubscription>()
                .HasOne(u => u.User)
                .WithMany(u => u.UserCommunitySubscriptions)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<CommunitySubscription>()
                .HasOne(u => u.Channel)
                .WithMany(u => u.ChannelCommunitySubscriptions)
                .HasForeignKey(c => c.ChannelId);

            modelBuilder.Entity<CommunitySubscription>()
                .HasIndex(u => new { u.UserId })
                .IncludeProperties(s => new { s.Date });

            modelBuilder.Entity<SubscriptionStatistic>()
                .HasKey(u => new { u.UserId, u.Date });

            modelBuilder.Entity<SubscriptionStatistic>()
                .HasOne(u => u.User)
                .WithMany(u => u.UserSubscriptionStatistics)
                .HasForeignKey(c => c.UserId);
        }
    }
}
