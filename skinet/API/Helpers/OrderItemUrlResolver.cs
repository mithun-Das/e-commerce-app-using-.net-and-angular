﻿using API.Dtos;
using AutoMapper;
using Core.Entities.OrderAggregate;
using Microsoft.Extensions.Configuration;

namespace API.Helpers;

public class OrderItemUrlResolver : IValueResolver<OrderItem, OrderItemDto, string>
{
    private readonly IConfiguration _config;
    public OrderItemUrlResolver(IConfiguration config)
    {
        this._config = config;
    }

    public string? Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
    {
        if(!string.IsNullOrEmpty(source.ItemOrdered.PictureUrl))
        {
            return this._config["ApiUrl"] + source.ItemOrdered.PictureUrl;
        }

        return null;
    }
}
