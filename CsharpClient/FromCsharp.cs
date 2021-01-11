using System;
using Xunit;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;

using Poc.Core;

namespace CsharpClient
{
    public class FromCsharp
    {
        [Fact]
        public void CanCreateProduct()
        {
            var product = Product.create("Test", 12m);
            Assert.Equal("Test", product.Name);
            Assert.Equal(12.0m, product.Price);
        }

        [Fact]
        public void CanCreateOrder()
        {
            var customer = Customer.create("John Doe");
            var product = Product.create("Banana", 0.9m);

            FSharpFunc<Unit, Order.OrderNumber> numberGenerator = FSharpFunc<Unit, Order.OrderNumber>.FromConverter((Unit) => Order.OrderNumber.NewOrderNumber("ABC-1234"));
            var order = Order.create(
                numberGenerator,
                customer,
                ListModule.OfSeq(new[] { product }),
                ListModule.OfSeq(new Payment[] {})
            );

            Assert.NotNull(order);
            Assert.Equal(0.900m, Order.total(order));
        }
    }
}
