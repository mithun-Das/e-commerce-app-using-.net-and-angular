using Core.Entities.OrderAggregate;
using System.Collections.Generic;
using System;

namespace API.Dtos;

public class OrderToReturnDto
{
    public int Id { get; set; }
    public string BuyerEmail { get; set; }
    public DateTime OrderDate { get; set; }
    public Address ShipToAddres { get; set; }
    public string DeliveryMethod { get; set; }
    public decimal ShippingPrice { get; set; }
    public IReadOnlyList<OrderItemDto> OrderItems { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; }
}
