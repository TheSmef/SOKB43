﻿using System;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Models.Entity.Account", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("UserId");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Login")
                        .IsUnique();

                    b.ToTable("Accounts", t =>
                        {
                            t.HasCheckConstraint("CH_Email_Account", "Email like '%@%.%'");
                        });

                    b.HasData(
                        new
                        {
                            UserId = new Guid("f0e290a9-9054-4ae7-af3b-08dad84feb5b"),
                            Email = "admin@admin.com",
                            Login = "admin",
                            Password = "$2a$11$iL7JhZiLPOC5Pj0qW8nsWek7/nBbdD/Hj3OGZtSIObkLjYdX3jqfO"
                        });
                });

            modelBuilder.Entity("Models.Entity.Contractor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("PhoneNumber")
                        .IsUnique();

                    b.ToTable("Contractors", t =>
                        {
                            t.HasCheckConstraint("CH_Email_Contractor", "Email like '%@%.%'");
                        });
                });

            modelBuilder.Entity("Models.Entity.Equipment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<string>("EquipmentCode")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<Guid>("TechnicalTaskId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("EquipmentCode")
                        .IsUnique();

                    b.HasIndex("OrderId");

                    b.HasIndex("TechnicalTaskId");

                    b.ToTable("Equipments");
                });

            modelBuilder.Entity("Models.Entity.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContractorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Sum")
                        .HasPrecision(15, 2)
                        .HasColumnType("decimal(15,2)");

                    b.HasKey("Id");

                    b.HasIndex("ContractorId");

                    b.ToTable("Orders", t =>
                        {
                            t.HasCheckConstraint("CH_Sum_Order", "Sum > 0");
                        });
                });

            modelBuilder.Entity("Models.Entity.Post", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("Salary")
                        .HasPrecision(15, 2)
                        .HasColumnType("decimal(15,2)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Posts", t =>
                        {
                            t.HasCheckConstraint("CH_Salary_Post", "Salary > 0");
                        });
                });

            modelBuilder.Entity("Models.Entity.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("AccountUserId");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = new Guid("0618a478-23fe-44f1-a3ac-7712d7f2bd75"),
                            AccountUserId = new Guid("f0e290a9-9054-4ae7-af3b-08dad84feb5b"),
                            Name = "Администратор"
                        });
                });

            modelBuilder.Entity("Models.Entity.Service", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("EquipmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ServiceType")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<decimal>("Sum")
                        .HasPrecision(15, 2)
                        .HasColumnType("decimal(15,2)");

                    b.Property<string>("WorkContent")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id");

                    b.HasIndex("EquipmentId");

                    b.ToTable("Services", t =>
                        {
                            t.HasCheckConstraint("CH_Sum_Service", "Sum > 0");
                        });
                });

            modelBuilder.Entity("Models.Entity.TechnicalTask", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(6000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("NameEquipment")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("TypeEquipmentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("NameEquipment")
                        .IsUnique();

                    b.HasIndex("TypeEquipmentId");

                    b.ToTable("TechnicalTasks");
                });

            modelBuilder.Entity("Models.Entity.TechnicalTest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<Guid>("EquipmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ExpectedConclusion")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("FactConclusion")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("Passed")
                        .HasColumnType("bit");

                    b.Property<string>("TestData")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("TestPriority")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("EquipmentId");

                    b.HasIndex("UserId");

                    b.ToTable("TechnicalTests");
                });

            modelBuilder.Entity("Models.Entity.Token", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("TokenStr")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.HasIndex("AccountUserId");

                    b.HasIndex("TokenStr")
                        .IsUnique();

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("Models.Entity.TypeEquipment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("TypesEquipment");
                });

            modelBuilder.Entity("Models.Entity.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("First_name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Last_name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Otch")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PassportNumber")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("nvarchar(6)");

                    b.Property<string>("PassportSeries")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("nvarchar(4)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.HasKey("Id");

                    b.HasIndex("PassportNumber", "PassportSeries")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("f0e290a9-9054-4ae7-af3b-08dad84feb5b"),
                            BirthDate = new DateTime(1993, 5, 9, 0, 0, 0, 0, DateTimeKind.Local),
                            First_name = "Админ",
                            Last_name = "Админ",
                            PassportNumber = "000000",
                            PassportSeries = "0000",
                            PhoneNumber = "88888888888"
                        });
                });

            modelBuilder.Entity("Models.Entity.UserPost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("PostId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Share")
                        .HasPrecision(3, 2)
                        .HasColumnType("decimal(3,2)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.ToTable("UserPosts", t =>
                        {
                            t.HasCheckConstraint("Ch_Share_UserPost", "Share > 0 AND Share <= 1");
                        });
                });

            modelBuilder.Entity("Models.Entity.Account", b =>
                {
                    b.HasOne("Models.Entity.User", "User")
                        .WithOne("Account")
                        .HasForeignKey("Models.Entity.Account", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Models.Entity.Equipment", b =>
                {
                    b.HasOne("Models.Entity.Order", "Order")
                        .WithMany("Equipments")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Models.Entity.TechnicalTask", "TechnicalTask")
                        .WithMany("Equipments")
                        .HasForeignKey("TechnicalTaskId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("TechnicalTask");
                });

            modelBuilder.Entity("Models.Entity.Order", b =>
                {
                    b.HasOne("Models.Entity.Contractor", "Contractor")
                        .WithMany("Orders")
                        .HasForeignKey("ContractorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contractor");
                });

            modelBuilder.Entity("Models.Entity.Role", b =>
                {
                    b.HasOne("Models.Entity.Account", "AccountUser")
                        .WithMany("Roles")
                        .HasForeignKey("AccountUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountUser");
                });

            modelBuilder.Entity("Models.Entity.Service", b =>
                {
                    b.HasOne("Models.Entity.Equipment", "Equipment")
                        .WithMany("Services")
                        .HasForeignKey("EquipmentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Equipment");
                });

            modelBuilder.Entity("Models.Entity.TechnicalTask", b =>
                {
                    b.HasOne("Models.Entity.TypeEquipment", "TypeEquipment")
                        .WithMany("TechnicalTasks")
                        .HasForeignKey("TypeEquipmentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("TypeEquipment");
                });

            modelBuilder.Entity("Models.Entity.TechnicalTest", b =>
                {
                    b.HasOne("Models.Entity.Equipment", "Equipment")
                        .WithMany("TechnicalTests")
                        .HasForeignKey("EquipmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Models.Entity.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Equipment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Models.Entity.Token", b =>
                {
                    b.HasOne("Models.Entity.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Models.Entity.UserPost", b =>
                {
                    b.HasOne("Models.Entity.Post", "Post")
                        .WithMany("UserPosts")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Models.Entity.User", "User")
                        .WithMany("UserPosts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Models.Entity.Account", b =>
                {
                    b.Navigation("Roles");
                });

            modelBuilder.Entity("Models.Entity.Contractor", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("Models.Entity.Equipment", b =>
                {
                    b.Navigation("Services");

                    b.Navigation("TechnicalTests");
                });

            modelBuilder.Entity("Models.Entity.Order", b =>
                {
                    b.Navigation("Equipments");
                });

            modelBuilder.Entity("Models.Entity.Post", b =>
                {
                    b.Navigation("UserPosts");
                });

            modelBuilder.Entity("Models.Entity.TechnicalTask", b =>
                {
                    b.Navigation("Equipments");
                });

            modelBuilder.Entity("Models.Entity.TypeEquipment", b =>
                {
                    b.Navigation("TechnicalTasks");
                });

            modelBuilder.Entity("Models.Entity.User", b =>
                {
                    b.Navigation("Account")
                        .IsRequired();

                    b.Navigation("UserPosts");
                });
#pragma warning restore 612, 618
        }
    }
}
