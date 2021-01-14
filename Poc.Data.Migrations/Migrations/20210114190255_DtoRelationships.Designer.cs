﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Poc.Data;

namespace Poc.Data.Migrations.Migrations
{
    [DbContext(typeof(OrderContext))]
    [Migration("20210114190255_DtoRelationships")]
    partial class DtoRelationships
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("Poc.Data.DTO+Customer+CustomerDto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Customer");
                });

            modelBuilder.Entity("Poc.Data.DTO+Order+OrderDto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CustomerId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("OrderNumber")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("Poc.Data.DTO+Order+OrderItemDto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("OrderDtoId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProductId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("OrderDtoId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderItem");
                });

            modelBuilder.Entity("Poc.Data.DTO+Payment+OrderPaymentDto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<int?>("OrderDtoId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PaymentType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PosReferenceNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("TxId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("OrderDtoId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("Poc.Data.DTO+Product+ProductDto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("Poc.Data.DTO+Order+OrderDto", b =>
                {
                    b.HasOne("Poc.Data.DTO+Customer+CustomerDto", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("Poc.Data.DTO+Order+OrderItemDto", b =>
                {
                    b.HasOne("Poc.Data.DTO+Order+OrderDto", null)
                        .WithMany("Items")
                        .HasForeignKey("OrderDtoId");

                    b.HasOne("Poc.Data.DTO+Product+ProductDto", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Poc.Data.DTO+Payment+OrderPaymentDto", b =>
                {
                    b.HasOne("Poc.Data.DTO+Order+OrderDto", null)
                        .WithMany("Payments")
                        .HasForeignKey("OrderDtoId");
                });

            modelBuilder.Entity("Poc.Data.DTO+Order+OrderDto", b =>
                {
                    b.Navigation("Items");

                    b.Navigation("Payments");
                });
#pragma warning restore 612, 618
        }
    }
}