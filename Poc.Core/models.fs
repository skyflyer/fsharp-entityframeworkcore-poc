namespace Poc.Core

open System
open WrappedString
open Validate

type ProductId = ProductId of int

type Product(id, name, price) =
    //TODO: id should be non optional here, e.g.
    do
        // TODO: this or `NonNegativeDecimal` wrapped type?
        if price < 0m then
            invalidArg "price" "Price must not be negative!"

    member this.Id: ProductId option = id
    member this.Name: string = Validate.ensureString50 name
    member this.Price: decimal = price

    new(name, price) = Product(None, name, price)

    override this.ToString() =
        sprintf "Product id: %A name: %s price: %M" this.Id this.Name this.Price

type CustomerId = CustomerId of int

type Customer(id, name) =
    member this.Id: CustomerId option = id
    member this.Name: String50 = name

    new(name: string) = Customer(None, Option.get (string50 name))


type OrderId = OrderId of int

type OrderStatus =
    | Created
    | Paid
    | Shipped
    | Delivered

type Payment(amount: decimal, ts: DateTimeOffset) =
    member this.Timestamp = ts
    member this.Amount = amount

type CashPayment(amount, ts, ref) =
    inherit Payment(amount, ts)
    with
        member this.PosReferenceNumber: String50 = string50 ref |> Option.get

type CreditCardType =
    | Visa
    | Mastercard
    | Amex

type CreditCardPayment(amount, ts, card, txId) =
    inherit Payment(amount, ts)
    with
        member this.Type: CreditCardType = card
        member this.TxId: String100 = string100 txId |> Option.get

type Order(id, customer, items: list<Product>) =
    do
        if items.IsEmpty then
            invalidArg "items" "Must not be empty list"

    let mutable status = Created
    let mutable payments : list<Payment> = []
    member this.Id: OrderId option = id
    member this.Customer: Customer = customer
    member this.Items = items
    member this.Status 
        with get() = status
        and set(value) = status <- value
    member this.Payments with get() = payments

    member this.AddPayment(p: Payment) =
        let itemsTotal =
          List.fold (fun acc (elem: Product) -> acc + elem.Price) 0m this.Items

        let paymentsTotal =
          p.Amount
          + List.fold (fun acc (elem: Payment) -> acc + elem.Amount) 0m this.Payments

        match paymentsTotal with
        | itemsTotal -> status <- Paid
        | paymentsTotal when paymentsTotal > itemsTotal -> failwith "Too much payment"

        payments <- (p :: payments)

    member this.MarkAsShipped() =
        match this.Status with
        | Paid -> this.Status <- Shipped
        | _ -> failwithf "Can not mark order as shipped in status %A" this.Status

    member this.MarkAsDelivered() =
        match this.Status with
        | Shipped -> this.Status <- Delivered
        | _ -> failwithf "Can not mark order as delivered in status %A" this.Status

    member this.Total with get() = List.fold (fun acc (elem:Product) -> acc + elem.Price) 0m items


