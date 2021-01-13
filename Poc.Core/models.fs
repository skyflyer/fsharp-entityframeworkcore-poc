namespace Poc.Core

open System
open WrappedString
open Validate


module Product =
    type ProductId = ProductId of int

    type Product =
        { Id: ProductId option
          Name: string
          Price: decimal }

        override this.ToString() =
            sprintf "Product id: %A name: %s price: %M" this.Id this.Name this.Price

    let createWithId id name price =
        { Id = id
          Name = ensureString50 name
          Price = ensureNonNegative price }

    let create (name:string) (price:decimal) = createWithId None name price

module Customer =
    type CustomerId = CustomerId of int

    type Customer =
        { Id: CustomerId option
          Name: String50 }

    let create name = { Id = None; Name = (string50 name) |> Option.get }


type OrderStatus =
    | Created
    | Paid
    | Shipped
    | Delivered

type PaymentId = PaymentId of int
type CashPayment =
    { Id: PaymentId option
      Timestamp: DateTimeOffset
      Amount: decimal
      PosReferenceNumber: string }

type CreditCardType =
    | Visa
    | Mastercard
    | Amex

type CreditCardPayment =
    { Id: PaymentId option
      Timestamp: DateTimeOffset
      Amount: decimal
      CardType: CreditCardType
      TxId: string }

type Payment =
    | CashPayment of CashPayment
    | CreditCardPayment of CreditCardPayment

module Order =

    type OrderId = OrderId of int
    type OrderNumber = OrderNumber of string

    type OrderNumberGenerator = unit -> OrderNumber
    type Order = {
        Id: OrderId option
        OrderNumber: OrderNumber
        Customer: Customer.Customer
        Items: Product.Product list
        Payments: Payment list
    }
    type OrderState =
        | Created of Order 
        | Paid of Order
        | Shipped of Order
        | Delivered of Order

    
    let total orderState =
        let order =
            match orderState with
            | Created order -> order
            | Paid order -> order
            | Shipped order -> order
            | Delivered order -> order

        order.Items |> List.sumBy (fun item -> item.Price)

    let totalPayments payments =
        payments
        |> List.sumBy (fun item ->
            match item with
            | CashPayment c -> c.Amount
            | CreditCardPayment c -> c.Amount)

    let create (generateOrderNumber:OrderNumberGenerator) customer items payments =
        // TODO: verify that payments not > total
        Created {
            Id = None
            OrderNumber = generateOrderNumber()
            Customer = customer
            Items = items
            Payments = payments }

    let addPayment (orderState: OrderState) (payment : Payment) =
        let orderTotal = total orderState

        let paymentAmount =
            match payment with
            | CashPayment c -> c.Amount
            | CreditCardPayment c -> c.Amount

        match orderState with 
        | Created order ->
            let payments = payment :: order.Payments
            let paymentIsComplete = (orderTotal = totalPayments payments)
            if totalPayments payments > orderTotal then
                failwith "Payment error"
            else if paymentIsComplete then
                Paid { order with Payments = payments }
            else
                Created { order with Payments = payments }
        | _ -> failwith "Payment error"
            

    let markOrderAsShipped orderState = 
        match orderState with
        | Paid order -> Shipped order
        | _ -> failwith "Not available"

    let markOrderAsDelivered orderState = 
        match orderState with
        | Shipped order -> Delivered order
        | _ -> failwith "Not available"
