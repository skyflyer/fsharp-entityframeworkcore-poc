# Scenario

* A customer is defined by an id and a name
* An order can be created for a new customer or an existing customer
    * order number must be unique and generated based on a DB sequence (e.g. "ORDER-234" where "ORDER-" is a custom prefix and "234" is the sequence nextval)
* An order must contain at least one item to be created
* Created order has a status of "Created"
* Cash payment or Credit card payment can be added to an order at any time
    * the sum of payment amounts must not exceed order value
    * each payment contains a date & time
    * cash payment must contain a POS reference number (alphanumeric string up to 50 chars)
    * credit card payment contains credit card type (Visa, Mastercard, Amex) and transaction id (alphanumeric string up to 100 chars)
* When the order is paid in full (amount of payments == order total) a domain event is fired and order transitions to a "Paid" state
* When "order paid" event is fired, an email notification is sent
* The order can move to status "Dispatched" only when it is fully paid. When the order is moved to a "Dispatched" state, an email notification is sent.
* The order can move to status "Delivered" only after it has been dispatched.

# Technology requirements

* SQL Server, non-nullable, unique, other constraints
* Order statuses need to be in a database
* Domain events can be handled with something like MediatR
* We need to prevent concurrent modifications (SQL rowversion)
