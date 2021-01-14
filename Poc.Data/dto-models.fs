namespace Poc.Data

open Poc.Core
open Poc.Core.Order
open System

module DTO =

    module Product = 
        [<CLIMutable>]
        type ProductDto = {
            Id: int
            Name: string
            Price: decimal
        }

        let productIdToInt (productId: Product.ProductId option) =
            productId
            |> Option.defaultValue (Product.ProductId 0)
            |> function
            | Product.ProductId x -> x

        let fromDomain =
            fun (product: Product.Product) ->
                { 
                    Id = productIdToInt product.Id
                    Name = product.Name
                    Price = product.Price
                }

    module Payment =

        type PaymentType =
            | CashPayment = 1
            | CreditCardPayment = 2

        type CreditCardType =
            | Visa = 1
            | Mastercard = 2
            | Amex = 3

        [<CLIMutable>]
        type OrderPaymentDto =
            { Id: int
              OrderId: int
              Amount: decimal
              PaymentType: int
              PosReferenceNumber: string option
              TxId: string option }

        let paymentIdToInt (paymentId: PaymentId option) =
            paymentId
            |> Option.defaultValue (PaymentId 0)
            |> function
            | PaymentId x -> x

        let paymentType payment =
            match payment with
            | CashPayment c -> int PaymentType.CashPayment
            | CreditCardPayment c -> int PaymentType.CreditCardPayment

        let fromDomain: (int -> Payment -> OrderPaymentDto) =
            fun orderId payment ->
                match payment with
                | CashPayment c ->
                    { Id = paymentIdToInt c.Id
                      OrderId = orderId
                      Amount = c.Amount
                      PaymentType = paymentType payment
                      PosReferenceNumber = Some c.PosReferenceNumber
                      TxId = None }
                | CreditCardPayment c ->
                    { Id = paymentIdToInt c.Id
                      OrderId = orderId
                      Amount = c.Amount
                      PaymentType = paymentType payment
                      PosReferenceNumber = None
                      TxId = Some c.TxId }

    module Customer =

        [<CLIMutable>]
        type CustomerDto = { Id: int; Name: string }


        let customerIdToInt (customerId: Customer.CustomerId option) =
            match Option.defaultValue (Customer.CustomerId 0) customerId with
            | Customer.CustomerId x -> x

        type MapCustomerToCustomerDto = (Customer.Customer -> CustomerDto)

        let fromDomain: MapCustomerToCustomerDto =
            fun customer ->
                { Id = customerIdToInt customer.Id
                  Name = WrappedString.value customer.Name }

    module Order =
        type OrderStatus =
            | CreatedOrder = 1
            | PaidOrder = 2
            | ShippedOrder = 3
            | DeliveredOrder = 4

        [<CLIMutable>]
        type OrderDto =
            { Id: int
              OrderNumber: string
              CustomerId: int
              Status: int
              Items: OrderItemDto list }

        and [<CLIMutable>] OrderItemDto =
            { Id: int
              OrderId: int
              ProductId: int
              Price: decimal }

        let orderNumberToString (OrderNumber (x)) = x

        let orderIdToInt (orderId: OrderId option) =
            match Option.defaultValue (OrderId 0) orderId with
            | OrderId x -> x

        let itemsFromDomain =
            fun orderId (items: Product.Product list) ->
                items
                |> List.map
                    (fun i ->
                        { Id = 0
                          OrderId = orderId
                          ProductId = Product.productIdToInt i.Id
                          Price = i.Price })

        let fromDomain =
            fun (customerId:int) orderState ->
                let order, status =
                    match orderState with
                    | Created o -> o, (int OrderStatus.CreatedOrder)
                    | _ -> failwith "notimpl"

                // OrderDto
                { Id = orderIdToInt (order.Id)
                  OrderNumber = string order.OrderNumber
                  CustomerId = customerId
                  Status = status
                  Items = itemsFromDomain 0 order.Items }

        let paymentsFromDomain =
            fun (orderId: int) (orderState: OrderState) ->
                let order =
                    match orderState with
                    | Created o -> o
                    | _ -> failwith "notimpl"

                let x =
                    match order.Payments with
                    | head :: tail -> head
                    | [] -> failwith "nogo"
                   
                order.Payments
                |> List.map (Payment.fromDomain orderId)
