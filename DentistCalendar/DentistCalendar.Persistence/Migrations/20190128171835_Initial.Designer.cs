﻿// <auto-generated />
using System;
using DentistCalendar.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DentistCalendar.Persistence.Migrations
{
    [DbContext(typeof(DentistCalendarDbContext))]
    [Migration("20190128171835_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DentistCalendar.Core.Entities.Admin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Adress")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<DateTime>("LicenseEndDate");

                    b.Property<DateTime>("LicenseStartDate");

                    b.Property<int>("LicenseType");

                    b.Property<string>("MobilePhone")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("ProfileImageUrl");

                    b.HasKey("Id");

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("DentistCalendar.Core.Entities.Appointment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("DentistId");

                    b.Property<int?>("DentistOfficeId");

                    b.Property<int?>("PatientId");

                    b.Property<int?>("ServiceId");

                    b.Property<DateTime>("StartDate");

                    b.HasKey("Id");

                    b.HasIndex("DentistId");

                    b.HasIndex("DentistOfficeId");

                    b.HasIndex("PatientId");

                    b.HasIndex("ServiceId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("DentistCalendar.Core.Entities.Dentist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Adress")
                        .IsRequired();

                    b.Property<string>("Description");

                    b.Property<string>("DoctorTitle");

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("MobilePhone")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("ProfileImageUrl");

                    b.HasKey("Id");

                    b.ToTable("Dentists");
                });

            modelBuilder.Entity("DentistCalendar.Core.Entities.DentistDentistOffice", b =>
                {
                    b.Property<int>("DentistId");

                    b.Property<int>("DentistOfficeId");

                    b.HasKey("DentistId", "DentistOfficeId");

                    b.HasIndex("DentistOfficeId");

                    b.ToTable("DentistDentistOffice");
                });

            modelBuilder.Entity("DentistCalendar.Core.Entities.DentistOffice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AboutUs");

                    b.Property<int?>("AdminId");

                    b.Property<string>("Adress")
                        .IsRequired();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Phone")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.ToTable("DentistOffices");
                });

            modelBuilder.Entity("DentistCalendar.Core.Entities.DentistService", b =>
                {
                    b.Property<int?>("DentistId");

                    b.Property<int?>("ServiceId");

                    b.HasKey("DentistId", "ServiceId");

                    b.HasIndex("ServiceId");

                    b.ToTable("DentistService");
                });

            modelBuilder.Entity("DentistCalendar.Core.Entities.Patient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Adress")
                        .IsRequired();

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("MobilePhone")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("ProfileImageUrl");

                    b.HasKey("Id");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("DentistCalendar.Core.Entities.Receptionist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Adress")
                        .IsRequired();

                    b.Property<int?>("DentistOfficeId");

                    b.Property<string>("LastName")
                        .IsRequired();

                    b.Property<string>("MobilePhone")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("ProfileImageUrl");

                    b.HasKey("Id");

                    b.HasIndex("DentistOfficeId");

                    b.ToTable("Receptionists");
                });

            modelBuilder.Entity("DentistCalendar.Core.Entities.Service", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("DentistOfficeId");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<double>("Price");

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.HasIndex("DentistOfficeId");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("DentistCalendar.Core.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccountType");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DentistCalendar.Core.Entities.Appointment", b =>
                {
                    b.HasOne("DentistCalendar.Core.Entities.Dentist", "Dentist")
                        .WithMany("Appointments")
                        .HasForeignKey("DentistId");

                    b.HasOne("DentistCalendar.Core.Entities.DentistOffice", "DentistOffice")
                        .WithMany()
                        .HasForeignKey("DentistOfficeId");

                    b.HasOne("DentistCalendar.Core.Entities.Patient", "Patient")
                        .WithMany("Appointments")
                        .HasForeignKey("PatientId");

                    b.HasOne("DentistCalendar.Core.Entities.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId");
                });

            modelBuilder.Entity("DentistCalendar.Core.Entities.DentistDentistOffice", b =>
                {
                    b.HasOne("DentistCalendar.Core.Entities.Dentist", "Dentist")
                        .WithMany("DentistDentistOffices")
                        .HasForeignKey("DentistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DentistCalendar.Core.Entities.DentistOffice", "DentistOffice")
                        .WithMany("DentistDentistOffices")
                        .HasForeignKey("DentistOfficeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DentistCalendar.Core.Entities.DentistOffice", b =>
                {
                    b.HasOne("DentistCalendar.Core.Entities.Admin", "Admin")
                        .WithMany("DentistOffices")
                        .HasForeignKey("AdminId");
                });

            modelBuilder.Entity("DentistCalendar.Core.Entities.DentistService", b =>
                {
                    b.HasOne("DentistCalendar.Core.Entities.Dentist", "Dentist")
                        .WithMany("DentistServices")
                        .HasForeignKey("DentistId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DentistCalendar.Core.Entities.Service", "Service")
                        .WithMany("DentistServices")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DentistCalendar.Core.Entities.Receptionist", b =>
                {
                    b.HasOne("DentistCalendar.Core.Entities.DentistOffice", "DentistOffice")
                        .WithMany("Receptionists")
                        .HasForeignKey("DentistOfficeId");
                });

            modelBuilder.Entity("DentistCalendar.Core.Entities.Service", b =>
                {
                    b.HasOne("DentistCalendar.Core.Entities.DentistOffice", "DentistOffice")
                        .WithMany("Services")
                        .HasForeignKey("DentistOfficeId");
                });
#pragma warning restore 612, 618
        }
    }
}
