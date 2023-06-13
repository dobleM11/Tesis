﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tesis.Models;

#nullable disable

namespace Tesis.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Tesis.Models.Empleado", b =>
                {
                    b.Property<string>("Run")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RolId")
                        .HasColumnType("int");

                    b.Property<int>("SeccionId")
                        .HasColumnType("int");

                    b.HasKey("Run");

                    b.HasIndex("RolId");

                    b.HasIndex("SeccionId");

                    b.ToTable("Empleados");
                });

            modelBuilder.Entity("Tesis.Models.Rol", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Tesis.Models.Seccion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Secciones");
                });

            modelBuilder.Entity("Tesis.Models.Sugerencia", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("FechaHora")
                        .HasColumnType("datetime2");

                    b.Property<string>("Texto")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Sugerencias");
                });

            modelBuilder.Entity("Tesis.Models.Turno", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Asistencia")
                        .HasColumnType("bit");

                    b.Property<DateTime>("FechaHora")
                        .HasColumnType("datetime2");

                    b.Property<int>("SeccionId")
                        .HasColumnType("int");

                    b.Property<string>("UsuarioRun")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("SeccionId");

                    b.HasIndex("UsuarioRun");

                    b.ToTable("Turnos");
                });

            modelBuilder.Entity("Tesis.Models.Usuario", b =>
                {
                    b.Property<string>("Run")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("Rolid")
                        .HasColumnType("int");

                    b.HasKey("Run");

                    b.HasIndex("Rolid");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("Tesis.Models.Empleado", b =>
                {
                    b.HasOne("Tesis.Models.Rol", "Rol")
                        .WithMany("Empleados")
                        .HasForeignKey("RolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tesis.Models.Seccion", "Seccion")
                        .WithMany("Empleados")
                        .HasForeignKey("SeccionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Rol");

                    b.Navigation("Seccion");
                });

            modelBuilder.Entity("Tesis.Models.Turno", b =>
                {
                    b.HasOne("Tesis.Models.Seccion", "Seccion")
                        .WithMany("Turnos")
                        .HasForeignKey("SeccionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tesis.Models.Usuario", "Usuario")
                        .WithMany("Turnos")
                        .HasForeignKey("UsuarioRun")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Seccion");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("Tesis.Models.Usuario", b =>
                {
                    b.HasOne("Tesis.Models.Rol", "Rol")
                        .WithMany("Usuarios")
                        .HasForeignKey("Rolid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Rol");
                });

            modelBuilder.Entity("Tesis.Models.Rol", b =>
                {
                    b.Navigation("Empleados");

                    b.Navigation("Usuarios");
                });

            modelBuilder.Entity("Tesis.Models.Seccion", b =>
                {
                    b.Navigation("Empleados");

                    b.Navigation("Turnos");
                });

            modelBuilder.Entity("Tesis.Models.Usuario", b =>
                {
                    b.Navigation("Turnos");
                });
#pragma warning restore 612, 618
        }
    }
}
