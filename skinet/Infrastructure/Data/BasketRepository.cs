﻿
using Core.Entities;
using Core.Interfaces;
using System.Threading.Tasks;

namespace Infrastructure.Data;

public class BasketRepository : IBasketRepository
{
    public Task<bool> DeleteBasketAsync(string basketId)
    {
        throw new System.NotImplementedException();
    }

    public Task<CustomerBasket> GetBasketAsync(string basketId)
    {
        throw new System.NotImplementedException();
    }

    public Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
        throw new System.NotImplementedException();
    }
}
