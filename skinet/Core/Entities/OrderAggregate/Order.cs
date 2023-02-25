
using System;
using System.Collections.Generic;

namespace Core.Entities.OrderAggregate;

public class Order : BaseEntity
{
    public Order() { }

    public Order(string buyerEmail, Address shipToAddress, DeliveryMethod deliveryMethod,
        IReadOnlyList<OrderItem> orderItems, decimal subTotal, string paymentIntentId)
    {
        this.BuyerEmail = buyerEmail;
        this.ShipToAddres = shipToAddress;
        this.DeliveryMethod = deliveryMethod;
        this.OrderItems = orderItems;
        this.SubTotal = subTotal;
        this.PaymentIntentId = paymentIntentId;
    }

    public string BuyerEmail { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public Address ShipToAddres { get; set; }
    public DeliveryMethod DeliveryMethod { get; set; }
    public IReadOnlyList<OrderItem> OrderItems { get; set; }
    public decimal SubTotal { get; set; }
    public OrderStatus Status { get; set; }
    public string PaymentIntentId { get; set; } = string.Empty;
    public decimal GetTotal()
    {
        return SubTotal + DeliveryMethod.Price;
    }
}
