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

    override __.OnConfiguring(optionsBuilder: DbContextOptionsBuilder) =
        optionsBuilder.UseSqlite(
            "Data source=demo.db",
            fun opts -> opts.MigrationsAssembly("Poc.Data.Migrations") |> ignore )
        |> ignore

    [<DefaultValue>]
    val mutable customers:DbSet<Customer.CustomerDto>

    [<DefaultValue>]
    val mutable products:DbSet<Product.ProductDto>

    [<DefaultValue>]
    val mutable orders:DbSet<Order.OrderDto>
    member x.Items
        with get() = x.Items
        and set v = x.Items <- v

    [<DefaultValue>]
    val mutable orderItems:DbSet<Order.OrderItemDto>
