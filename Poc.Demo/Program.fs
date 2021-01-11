open System
open Poc.Core
open Poc.Core.Product


[<EntryPoint>]
let main argv =
    let p1 = Product.create "Apple" 12.0m

    let createOrder =
        Order.create (fun unit -> Order.OrderNumber "ABC-123")

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
                { Timestamp = DateTimeOffset.Now
                  Amount = 14m
                  PosReferenceNumber = "ABC097" })

    printfn "Order: %A" paidOrder

    let shippedOrder = Order.markOrderAsShipped paidOrder
    printfn "Shipped order: %A" shippedOrder

    let delivered = Order.markOrderAsDelivered shippedOrder
    printfn "Delivered order: %A" delivered

    0 // return an integer exit code
