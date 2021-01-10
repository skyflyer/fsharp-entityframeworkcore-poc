open System
open Poc.Core
open Poc.Core.WrappedString
open Poc.Core.Validate

type PositiveInt = private PInt of int with
    static member FromInt i =
        if i <= 0 then invalidArg "value" "Must be positive!"
        else PInt i

module NonNegativeDecimal = 
    type T = NonNegativeDecimal of decimal

    let create d =
        // if d >= 0m then d
        // else invalidArg "value" "The value must not be negative!"
        ensureNonNegative d

[<EntryPoint>]
let main argv =
    Validate.ensureNonNegative 0m |> ignore
    Validate.ensureNonNegative 1m |> ignore
    Validate.ensureNonNegative 12.2m |> ignore

    let d1 = NonNegativeDecimal.create 2.2m
    try
        NonNegativeDecimal.create -2.2m |> ignore
    with
        | _ -> ()

    let p1 = Product("Apple", 12.0m)
    try
        Product("Fail", -2.0m) |> ignore
        printfn "== ERROR == Should not happen!"
    with
        | ex -> printfn "Price with negative price failed: %s" ex.Message
    let p3 = Product(Some (ProductId 2), "Banana", 2.0m)

    // printfn "P1: %A" p1
    // printfn "P3: %A" p3

    let c = Customer("John Doe")
    let o = Order(None, c, [p1])

    try
        o.MarkAsShipped()
        printfn "== ERROR == should fail before"
    with
        | ex -> printfn "OK - can not mark as shipped yet: %s" ex.Message


    o.AddPayment(CashPayment(14m, DateTimeOffset.Now, "ABC097"))
    printfn "Order status: %A" o.Status

    o.MarkAsShipped()
    o.MarkAsDelivered()
    printfn "Order status: %A" o.Status
    printfn "Order total: %.2f" o.Total

    0 // return an integer exit code