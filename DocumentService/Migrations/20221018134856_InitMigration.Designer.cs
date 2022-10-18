﻿// <auto-generated />
using System;
using DocumentService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DocumentService.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20221018134856_InitMigration")]
    partial class InitMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("DocumentService.Models.DocumentMetadataModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("DocumentMetadata");
                });

            modelBuilder.Entity("DocumentService.Models.MetadataTagsLinkModel", b =>
                {
                    b.Property<Guid>("DocumentMetadataModelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TagId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("DocumentMetadataModelId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("MetadataTags");
                });

            modelBuilder.Entity("DocumentService.Models.TagModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("DocumentService.Models.MetadataTagsLinkModel", b =>
                {
                    b.HasOne("DocumentService.Models.DocumentMetadataModel", "DocumentMetadataModel")
                        .WithMany("MetadataTagsLinks")
                        .HasForeignKey("DocumentMetadataModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DocumentService.Models.TagModel", "Tag")
                        .WithMany("MetadataTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DocumentMetadataModel");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("DocumentService.Models.DocumentMetadataModel", b =>
                {
                    b.Navigation("MetadataTagsLinks");
                });

            modelBuilder.Entity("DocumentService.Models.TagModel", b =>
                {
                    b.Navigation("MetadataTags");
                });
#pragma warning restore 612, 618
        }
    }
}