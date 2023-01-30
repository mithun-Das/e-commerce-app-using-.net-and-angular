using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers;

public class BasketController : BaseApiController
{
    private readonly IBasketRepository _basketRepository;
    private readonly IMapper _mapper;

    public BasketController(IBasketRepository basketRepository, IMapper mapper)
    {
        this._basketRepository = basketRepository;
        this._mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
    {
        var data = await this._basketRepository.GetBasketAsync(id);

        return Ok(data ?? new CustomerBasket(id));
    }

    [HttpPost]
    public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto customerBasketDto)
    {
        var basket = this._mapper.Map<CustomerBasket>(customerBasketDto);

        var updateBasket = await this._basketRepository.UpdateBasketAsync(basket);

        return Ok(updateBasket);
    }

    [HttpDelete]
    public async Task DeleteBasket(string id)
    {
        await this._basketRepository.DeleteBasketAsync(id);
    }
}
