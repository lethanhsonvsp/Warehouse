﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Warehouse.Db;

#nullable disable

namespace Warehouse.Migrations
{
    [DbContext(typeof(WarehouseDbContext))]
    [Migration("20250314014446_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.3");

            modelBuilder.Entity("Warehouse.Model.Location", b =>
                {
                    b.Property<string>("Location_ID")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Parent_Location_ID")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Location_ID");

                    b.HasIndex("Parent_Location_ID");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("Warehouse.Model.Pallet", b =>
                {
                    b.Property<string>("Pallet_ID")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime?>("Creation_Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Robot_ID")
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Size")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Type")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Pallet_ID");

                    b.HasIndex("Robot_ID");

                    b.ToTable("Pallets");
                });

            modelBuilder.Entity("Warehouse.Model.Pallet_Location", b =>
                {
                    b.Property<string>("Pallet_ID")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)")
                        .HasColumnOrder(0);

                    b.Property<DateTime>("Time_In")
                        .HasColumnType("datetime2")
                        .HasColumnOrder(1);

                    b.Property<string>("Location_ID")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("Time_Out")
                        .HasColumnType("datetime2");

                    b.HasKey("Pallet_ID", "Time_In");

                    b.HasIndex("Location_ID");

                    b.ToTable("Pallet_Locations");
                });

            modelBuilder.Entity("Warehouse.Model.Robot", b =>
                {
                    b.Property<string>("Robot_ID")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Robot_ID");

                    b.ToTable("Robots");
                });

            modelBuilder.Entity("Warehouse.Model.Location", b =>
                {
                    b.HasOne("Warehouse.Model.Location", "Parent_Location")
                        .WithMany("Child_Locations")
                        .HasForeignKey("Parent_Location_ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Parent_Location");
                });

            modelBuilder.Entity("Warehouse.Model.Pallet", b =>
                {
                    b.HasOne("Warehouse.Model.Robot", "Robot")
                        .WithMany("Pallets")
                        .HasForeignKey("Robot_ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Robot");
                });

            modelBuilder.Entity("Warehouse.Model.Pallet_Location", b =>
                {
                    b.HasOne("Warehouse.Model.Location", "Location")
                        .WithMany("Pallet_Locations")
                        .HasForeignKey("Location_ID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Warehouse.Model.Pallet", "Pallet")
                        .WithMany("Pallet_Locations")
                        .HasForeignKey("Pallet_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");

                    b.Navigation("Pallet");
                });

            modelBuilder.Entity("Warehouse.Model.Location", b =>
                {
                    b.Navigation("Child_Locations");

                    b.Navigation("Pallet_Locations");
                });

            modelBuilder.Entity("Warehouse.Model.Pallet", b =>
                {
                    b.Navigation("Pallet_Locations");
                });

            modelBuilder.Entity("Warehouse.Model.Robot", b =>
                {
                    b.Navigation("Pallets");
                });
#pragma warning restore 612, 618
        }
    }
}
