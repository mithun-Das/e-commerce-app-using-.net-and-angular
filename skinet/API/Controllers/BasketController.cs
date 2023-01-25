using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers;

public class BasketController : BaseApiController
{
    private readonly IBasketRepository _basketRepository;

    public BasketController(IBasketRepository basketRepository)
    {
        this._basketRepository = basketRepository;
    }

    [HttpGet]
    public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
    {
        var data = await this._basketRepository.GetBasketAsync(id);

        return Ok(data ?? new CustomerBasket(id));
    }

    [HttpPost]
    public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasket customerBasket)
    {
        var updateBasket = await this._basketRepository.UpdateBasketAsync(customerBasket);

        return Ok(updateBasket);
    }

    [HttpDelete]
    public async Task DeleteBasket(string id)
    {
        await this._basketRepository.DeleteBasketAsync(id);
    }
}
