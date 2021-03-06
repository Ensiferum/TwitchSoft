﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TwitchSoft.Shared.Database;

namespace TwitchSoft.Shared.Migrations
{
    [DbContext(typeof(TwitchDbContext))]
    [Migration("20200107172816_drop_messages")]
    partial class drop_messages
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TwitchSoft.Shared.Database.Models.CommunitySubscription", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("ChannelId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<int>("GiftCount")
                        .HasColumnType("int");

                    b.Property<int>("SubscriptionPlan")
                        .HasColumnType("int");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("UserId")
                        .HasAnnotation("SqlServer:Include", new[] { "Date" });

                    b.ToTable("CommunitySubscriptions");
                });

            modelBuilder.Entity("TwitchSoft.Shared.Database.Models.Subscription", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("ChannelId")
                        .HasColumnType("bigint");

                    b.Property<long?>("GiftedBy")
                        .HasColumnType("bigint");

                    b.Property<int>("Months")
                        .HasColumnType("int");

                    b.Property<DateTime>("SubscribedTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("SubscriptionPlan")
                        .HasColumnType("int");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<byte>("UserType")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.HasIndex("ChannelId");

                    b.HasIndex("GiftedBy");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("UserId")
                        .HasAnnotation("SqlServer:Include", new[] { "SubscribedTime" });

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("TwitchSoft.Shared.Database.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<bool>("JoinChannel")
                        .HasColumnType("bit");

                    b.Property<bool>("TrackMessages")
                        .HasColumnType("bit");

                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(60)")
                        .HasMaxLength(60);

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TwitchSoft.Shared.Database.Models.UserBan", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("BannedTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("BanType")
                        .HasColumnType("int");

                    b.Property<long>("ChannelId")
                        .HasColumnType("bigint");

                    b.Property<int?>("Duration")
                        .HasColumnType("int");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(140)")
                        .HasMaxLength(140);

                    b.HasKey("UserId", "BannedTime");

                    b.HasIndex("ChannelId");

                    b.HasIndex("UserId")
                        .HasAnnotation("SqlServer:Include", new[] { "BannedTime" });

                    b.ToTable("UserBans");
                });

            modelBuilder.Entity("TwitchSoft.Shared.Database.Models.CommunitySubscription", b =>
                {
                    b.HasOne("TwitchSoft.Shared.Database.Models.User", "Channel")
                        .WithMany("ChannelCommunitySubscriptions")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TwitchSoft.Shared.Database.Models.User", "User")
                        .WithMany("UserCommunitySubscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TwitchSoft.Shared.Database.Models.Subscription", b =>
                {
                    b.HasOne("TwitchSoft.Shared.Database.Models.User", "Channel")
                        .WithMany("ChannelSubscriptions")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TwitchSoft.Shared.Database.Models.User", "GiftedByUser")
                        .WithMany("UserSubscriptionGifts")
                        .HasForeignKey("GiftedBy");

                    b.HasOne("TwitchSoft.Shared.Database.Models.User", "User")
                        .WithMany("UserSubscriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TwitchSoft.Shared.Database.Models.UserBan", b =>
                {
                    b.HasOne("TwitchSoft.Shared.Database.Models.User", "Channel")
                        .WithMany("ChannelUserBans")
                        .HasForeignKey("ChannelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TwitchSoft.Shared.Database.Models.User", "User")
                        .WithMany("UserUserBans")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
