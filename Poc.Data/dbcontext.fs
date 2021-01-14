namespace Poc.Data

open System
open Microsoft.EntityFrameworkCore
open System.Collections.ObjectModel
open Poc.Data.DTO

type OrderContext =
    inherit DbContext

    new() = { inherit DbContext() }
    new(options: DbContextOptions<OrderContext>) = { inherit DbContext(options) }

    override __.OnModelCreating modelBuilder = 
        modelBuilder.Entity<Order.OrderDto>().ToTable("Order") |> ignore

        modelBuilder.Entity<Order.OrderItemDto>().ToTable("OrderItem") |> ignore
        
        modelBuilder.Entity<Customer.CustomerDto>().ToTable("Customer") |> ignore
        
        modelBuilder.Entity<Product.ProductDto>().ToTable("Product") |> ignore
        
        modelBuilder.Entity<Payment.OrderPaymentDto>().ToTable("Payments") |> ignore

    override __.OnConfiguring(optionsBuilder: DbContextOptionsBuilder) =
        optionsBuilder.UseSqlite(
            "Data source=demo.db",
            fun opts -> opts.MigrationsAssembly("Poc.Data.Migrations") |> ignore )
        |> ignore

    [<DefaultValue>]
    val mutable customers:DbSet<Customer.CustomerDto>
    member this.Customers
        with get() = this.customers
        and set v = this.customers <- v

    [<DefaultValue>]
    val mutable products:DbSet<Product.ProductDto>
    member this.Products
        with get() = this.products
        and set v = this.products <- v


    [<DefaultValue>]
    val mutable orders:DbSet<Order.OrderDto>
    member this.Orders
        with get() = this.orders
        and set v = this.orders <- v

    [<DefaultValue>]
    val mutable orderItems:DbSet<Order.OrderItemDto>
    member this.OrderItems
        with get() = this.orderItems
        and set v = this.orderItems <- v
