
using Core.Entities;
using Core.Interfaces;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Data;

public class BasketRepository : IBasketRepository
{
    private readonly IDatabase _database;

    public BasketRepository(IConnectionMultiplexer redis)
    {
        this._database = redis.GetDatabase();
    }

    public async Task<bool> DeleteBasketAsync(string basketId)
    {
        return await this._database.KeyDeleteAsync(basketId);
    }

    public async Task<CustomerBasket> GetBasketAsync(string basketId)
    {
        var data = await this._database.StringGetAsync(basketId);

        var result = data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(data);

        return result;
    }

    public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
    {
        var created = await this._database
                                .StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), TimeSpan.FromDays(30));

        if (!created) return null;

        return await GetBasketAsync(basket.Id);
    }
}
