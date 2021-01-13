open System
open Poc.Core
open Poc.Core.Product
open Poc.Data.DTO

let createOrder =
    Order.create (fun unit -> Order.OrderNumber "ABC-123")

module Play =
    let sandbox : (unit -> unit) =
        fun () ->
            let p1 = Product.create "Apple" 12.0m

            try
                Product.create "Fail" -2.0m |> ignore
                printfn "== ERROR == Should not happen!"
            with ex -> printfn "Price with negative price failed: %s" ex.Message

            let p3 =
                Product.createWithId (Some(ProductId 2)) "Banana" 2.0m

            let customer = Customer.create "John Doe"
            let order = createOrder customer [ p1; p3 ] []
            printfn "Created order: %A" order
            printfn "        Total: %.2f" (Order.total order)

            try
                Order.markOrderAsShipped order |> ignore
                printfn "== ERROR == should fail before"
            with ex -> printfn "OK - can not mark as shipped yet: %s" ex.Message


            let paidOrder =
                Order.addPayment
                    order
                    (CashPayment
                        { Id = None
                          Timestamp = DateTimeOffset.Now
                          Amount = 14m
                          PosReferenceNumber = "ABC097" })

            printfn "Order: %A" paidOrder

            let shippedOrder = Order.markOrderAsShipped paidOrder
            printfn "Shipped order: %A" shippedOrder

            let delivered = Order.markOrderAsDelivered shippedOrder
            printfn "Delivered order: %A" delivered

[<EntryPoint>]
let main argv =
    let product = Product.createWithId (Some (ProductId 1)) "Apple" 12.0m
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

    // Save order
    let savedCustomerDto = { Customer.fromDomain customer with Id = 772 }
    // let orderDto, orderItemDtos, orderPaymentDtos = Order.fromDomain order
    let savedOrderDto = { (Order.fromDomain savedCustomerDto.Id order) with Id = 44 }

    
    let orderItemDtos = Order.itemsFromDomain savedOrderDto.Id order 
    let paymentItemDtos = Order.paymentsFromDomain savedOrderDto.Id order
    printfn "%A" (savedCustomerDto, savedOrderDto, orderItemDtos, paymentItemDtos)

    0 // return an integer exit code
