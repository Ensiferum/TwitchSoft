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
    [Migration("20210131101113_change_indexes")]
    partial class change_indexes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

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
                        .IncludeProperties(new[] { "Date" });

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

                    b.HasIndex("ChannelId")
                        .IncludeProperties(new[] { "SubscribedTime" });

                    b.HasIndex("GiftedBy");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("UserId")
                        .IncludeProperties(new[] { "SubscribedTime" });

                    b.ToTable("Subscriptions");
                });

            modelBuilder.Entity("TwitchSoft.Shared.Database.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsBanned")
                        .HasColumnType("bit");

                    b.Property<bool>("JoinChannel")
                        .HasColumnType("bit");

                    b.Property<string>("Username")
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("JoinChannel");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TwitchSoft.Shared.Database.Models.UserBan", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<int>("BanType")
                        .HasColumnType("int");

                    b.Property<DateTime>("BannedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Channel")
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.Property<int?>("Duration")
                        .HasColumnType("int");

                    b.Property<string>("Reason")
                        .HasMaxLength(140)
                        .HasColumnType("nvarchar(140)");

                    b.Property<string>("UserName")
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.HasKey("Id");

                    b.HasIndex("Channel");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("UserName");

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

                    b.Navigation("Channel");

                    b.Navigation("User");
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

                    b.Navigation("Channel");

                    b.Navigation("GiftedByUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TwitchSoft.Shared.Database.Models.User", b =>
                {
                    b.Navigation("ChannelCommunitySubscriptions");

                    b.Navigation("ChannelSubscriptions");

                    b.Navigation("UserCommunitySubscriptions");

                    b.Navigation("UserSubscriptionGifts");

                    b.Navigation("UserSubscriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
