﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Server.Database.Implementations;

namespace Server.Migrations
{
    [DbContext(typeof(TweetDbContext))]
    [Migration("20161107134626_TweetMigration1")]
    partial class TweetMigration1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("Core.Models.Tweet", b =>
                {
                    b.Property<string>("Id");

                    b.Property<DateTime>("Date");

                    b.Property<string>("Key");

                    b.Property<string>("Language");

                    b.Property<double>("Latitude");

                    b.Property<double>("Longitude");

                    b.Property<int>("Sentiment");

                    b.Property<string>("Text");

                    b.HasKey("Id");

                    b.ToTable("Tweets");
                });
        }
    }
}
