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
              Amount: decimal
              PaymentType: int
              PosReferenceNumber: string
              TxId: string }

        let paymentIdToInt (paymentId: PaymentId option) =
            paymentId
            |> Option.defaultValue (PaymentId 0)
            |> function
            | PaymentId x -> x

        let paymentType payment =
            match payment with
            | CashPayment c -> int PaymentType.CashPayment
            | CreditCardPayment c -> int PaymentType.CreditCardPayment

        let fromDomain: (Payment -> OrderPaymentDto) =
            fun payment ->
                match payment with
                | CashPayment c ->
                    { Id = paymentIdToInt c.Id
                      Amount = c.Amount
                      PaymentType = paymentType payment
                      PosReferenceNumber = c.PosReferenceNumber
                      TxId = null }
                | CreditCardPayment c ->
                    { Id = paymentIdToInt c.Id
                      Amount = c.Amount
                      PaymentType = paymentType payment
                      PosReferenceNumber = null
                      TxId = c.TxId }

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
              Customer: Customer.CustomerDto
              Status: int
              Items: Collections.Generic.List<OrderItemDto>
              Payments: Collections.Generic.List<Payment.OrderPaymentDto> }

        and [<CLIMutable>] OrderItemDto =
            { Id: int
              Product: Product.ProductDto
              Title: string
              Price: decimal }

        let orderNumberToString (OrderNumber (x)) = x

        let orderIdToInt (orderId: OrderId option) =
            match Option.defaultValue (OrderId 0) orderId with
            | OrderId x -> x

        let itemsFromDomain =
            fun (items: Product.Product list) ->
                items
                |> List.map
                    (fun i ->
                        { Id = 0
                          Product = Product.fromDomain i
                          Title = i.Name
                          Price = i.Price })
                |> ResizeArray<_>

        let paymentsFromDomain =
            fun (payments: Payment list) ->
                payments
                |> List.map Payment.fromDomain
                |> ResizeArray<_>

        let fromDomain =
            fun orderState ->
                let order, status =
                    match orderState with
                    | Created o -> o, (int OrderStatus.CreatedOrder)
                    | _ -> failwith "notimpl"

                // OrderDto
                { Id = orderIdToInt (order.Id)
                  OrderNumber = string order.OrderNumber
                  Customer = Customer.fromDomain order.Customer
                  Status = status
                  Items = (itemsFromDomain order.Items)
                  Payments = (paymentsFromDomain order.Payments)
                  }

