using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Server.Database.Implementations;

namespace Server.Migrations
{
    [DbContext(typeof(TweetDbContext))]
    [Migration("20161107194327_TwitterMigration2")]
    partial class TwitterMigration2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("Core.Models.Tweet", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Key");

                    b.Property<string>("Language");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<int>("Sentiment");

                    b.Property<string>("Text");

                    b.Property<string>("TweetIdentifier");

                    b.HasKey("Id");

                    b.ToTable("Tweets");
                });
        }
    }
}
