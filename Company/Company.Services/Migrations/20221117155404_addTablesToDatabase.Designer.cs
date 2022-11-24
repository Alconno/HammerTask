﻿// <auto-generated />
using System;
using Company.Services.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Company.Services.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20221117155404_addTablesToDatabase")]
    partial class addTablesToDatabase
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Company.Models.Models.Department", b =>
                {
                    b.Property<int>("departmentNo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("departmentLocation")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("departmentName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("departmentNo");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("Company.Models.Models.Employee", b =>
                {
                    b.Property<int>("employeeNo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("EdepartmentNo")
                        .HasColumnType("int");

                    b.Property<int>("Salary")
                        .HasColumnType("int");

                    b.Property<int?>("departmentNo")
                        .HasColumnType("int");

                    b.Property<string>("employeeName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("lastModifyDate")
                        .HasColumnType("datetime2");

                    b.HasKey("employeeNo");

                    b.HasIndex("departmentNo");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Company.Models.Models.Login", b =>
                {
                    b.Property<int>("loginNo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("loginPassword")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("loginUserName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("loginNo");

                    b.ToTable("Logins");
                });

            modelBuilder.Entity("Company.Models.Models.Employee", b =>
                {
                    b.HasOne("Company.Models.Models.Department", "department")
                        .WithMany()
                        .HasForeignKey("departmentNo");

                    b.Navigation("department");
                });
#pragma warning restore 612, 618
        }
    }
}