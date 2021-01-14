namespace Poc.Demo

open System
open Poc.Core
open Poc.Core.Product
open Poc.Data
open Poc.Data.DTO
open Microsoft.EntityFrameworkCore

module Main =
    let createOrder =
        Order.create (fun unit -> Order.OrderNumber "ABC-123")

    let dbContext = 
        fun () ->
            printfn "Creating DB context..."
            let opts = new DbContextOptionsBuilder<OrderContext>()
            opts.UseSqlite("Data source=demo.db",  (fun o -> o.MigrationsAssembly("Poc.Data.Migrations") |> ignore)) |> ignore
            new OrderContext(opts.Options)
        

    [<EntryPoint>]
    let main argv =
        let product = Product.createWithId (Some (ProductId 0)) "Apple" 12.0m
        let customer = Customer.create "John Doe"

        let order =
            createOrder
                customer
                [ product ]
                [ CashPayment
                    { Id = None
                      Timestamp = DateTimeOffset.Now
                      Amount = 12.0m
                      PosReferenceNumber = "ABC123" } ]

        let migrateDb = 
            use db = dbContext()
            db.Database.Migrate() |> ignore

        let exampleOrder = 
            use db = dbContext()
            // Save order
            printfn "About to save an order: %A" (Order.fromDomain order)
            db.orders.Add(Order.fromDomain order) |> ignore
            db.SaveChanges() |> ignore

        let anotheExample =
            let queryCustomerAndProduct = 
                use db = dbContext()
                printfn "About to save another order..."
                // Query data with the same customer and product in the DB
                let customer = 
                    query {
                        for customer in db.Customers do
                        head
                    } 
                    |> Customer.toDomain
                
                let product = 
                    query {
                        for product in db.Products do
                        head
                    }
                    |> Product.toDomain
                (customer, product)

            let loadedCustomer, loadedProduct = queryCustomerAndProduct
            use db = dbContext() // new DbContext -- see entity tracking issues

            // create and save new order
            createOrder
                loadedCustomer
                [loadedProduct]
                [CashPayment { Id=None; Timestamp=DateTimeOffset.Now; Amount=12m; PosReferenceNumber="ABC1234"}]
            |> Order.fromDomain
            |> fun dto ->
                printfn "Saving order... %A" dto
                dto
            |> fun dto -> // very ugly workaround for attaching existing entities - not production code by any means
                if dto.Customer.Id <> 0 then
                    db.Attach(dto.Customer)
                    |> fun e -> e.State <- EntityState.Unchanged
                dto.Items
                |> Seq.iter (fun i ->
                    if i.Product.Id <> 0 then
                        db.Attach(i.Product)
                        |> fun e -> e.State <- EntityState.Unchanged)
                dto
            |> db.Orders.Add
            |> ignore

            db.SaveChanges() |> ignore


        0 // return an integer exit code
