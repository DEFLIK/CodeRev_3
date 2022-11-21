﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using UserService.DAL;

namespace UserService.DAL.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20221118150753_AddField-ProgrammingLanguage-To-InterviewTable")]
    partial class AddFieldProgrammingLanguageToInterviewTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.15")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("UserService.DAL.Entities.Interview", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("InterviewDurationMs")
                        .HasColumnType("bigint");

                    b.Property<string>("InterviewText")
                        .HasColumnType("text");

                    b.Property<string>("ProgrammingLanguage")
                        .HasColumnType("text");

                    b.Property<string>("Vacancy")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Interviews");
                });

            modelBuilder.Entity("UserService.DAL.Entities.InterviewSolution", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("AverageGrade")
                        .HasColumnType("integer");

                    b.Property<long>("EndTimeMs")
                        .HasColumnType("bigint");

                    b.Property<Guid>("InterviewId")
                        .HasColumnType("uuid");

                    b.Property<int>("InterviewResult")
                        .HasColumnType("integer");

                    b.Property<bool>("IsSubmittedByCandidate")
                        .HasColumnType("boolean");

                    b.Property<string>("ReviewerComment")
                        .HasColumnType("text");

                    b.Property<long>("StartTimeMs")
                        .HasColumnType("bigint");

                    b.Property<long>("TimeToCheckMs")
                        .HasColumnType("bigint");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("InterviewSolutions");
                });

            modelBuilder.Entity("UserService.DAL.Entities.InterviewTask", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("InterviewId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TaskId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("InterviewTasks");
                });

            modelBuilder.Entity("UserService.DAL.Entities.Invitation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("ExpiredAt")
                        .HasColumnType("bigint");

                    b.Property<Guid>("InterviewId")
                        .HasColumnType("uuid");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Invitations");
                });

            modelBuilder.Entity("UserService.DAL.Entities.Task", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("StartCode")
                        .HasColumnType("text");

                    b.Property<string>("TaskText")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("UserService.DAL.Entities.TaskSolution", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Grade")
                        .HasColumnType("integer");

                    b.Property<Guid>("InterviewSolutionId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsDone")
                        .HasColumnType("boolean");

                    b.Property<Guid>("TaskId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("TaskSolutions");
                });

            modelBuilder.Entity("UserService.DAL.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("FullName")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
