using Infrastructure.Data;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;

    public ProductsController(IProductRepository productRepository) 
    {
        _productRepository = productRepository;
    }

    #region[GET Requests]

    /// <Summary>
    /// Get The Product List
    /// </Summary>
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetProducts()
    {
        var products = await _productRepository.GetProductsAsync();

        return Ok(products);
    }

    /// <Summary>
    /// Get The Specific Product
    /// </Summary>
    /// <param name="id"></param>
    /// <returns> Product with specific id </returns>
    [HttpGet("{id}")]
    public async  Task<ActionResult<string>> GetProduct(int id)
    {
        var product = await _productRepository.GetProductByIdAsync(id);

        return Ok(product);
    }

    /// <Summary>
    /// Get The Product Brands
    /// </Summary>
    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
    {
        var productBrands = await _productRepository.GetProductBrandsAsync();

        return Ok(productBrands);
    }

    /// <Summary>
    /// Get The Product Types
    /// </Summary>
    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductTypes()
    {
        var productTypes = await _productRepository.GetProductTypesAsync();

        return Ok(productTypes);
    }

    #endregion
}
