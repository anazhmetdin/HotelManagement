﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.Configuration;
using DB.Models;
using Microsoft.EntityFrameworkCore;

namespace DB.Context;

public partial class HotelContext : DbContext
{
    public HotelContext()
    {
    }

    public HotelContext(DbContextOptions<HotelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<frontend> frontends { get; set; }

    public virtual DbSet<kitchen> kitchens { get; set; }

    public virtual DbSet<reservation> reservations { get; set; }

    public virtual DbSet<guest> guests { get; set; }

    public virtual DbSet<card> cards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["HotelManagement"].ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<frontend>(entity =>
        {
            entity.HasKey(e => e.user_name).HasName("PK_Table");

            entity.ToTable("frontend");

            entity.Property(e => e.user_name).HasMaxLength(50);
            entity.Property(e => e.pass_word)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<kitchen>(entity =>
        {
            entity.HasKey(e => e.user_name).HasName("PK__kitchen__7628B51D2FDAF8B5");

            entity.ToTable("kitchen");

            entity.Property(e => e.user_name).HasMaxLength(50);
            entity.Property(e => e.pass_word)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__reservat__3214EC0721710042");

            entity.ToTable("reservation");

            entity.Property(e => e.arrival_time).HasColumnType("date");
            entity.Property(e => e.leaving_time).HasColumnType("date");
            entity.Property(e => e.payment_type)
                .IsRequired()
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.room_floor)
                .IsRequired()
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.room_number)
                .IsRequired()
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.room_type)
                .IsRequired()
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<guest>(entity =>
        {
            entity.HasKey(e => e.SSN);

            entity.ToTable("guest");

            entity.Property(e => e.SSN)
                .ValueGeneratedNever();
            entity.Property(e => e.birth_day)
                .IsRequired();
            entity.Property(e => e.city).IsRequired();
            entity.Property(e => e.email_address).IsRequired();
            entity.Property(e => e.first_name)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.gender)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.last_name)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.apt_suite)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.state)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.street_address).IsRequired();
            entity.Property(e => e.zip_code)
                .IsRequired()
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.phone_number)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<card>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.card_cvc)
                .IsRequired()
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.card_exp)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.card_number)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.card_type)
                .IsRequired()
                .HasMaxLength(10)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}