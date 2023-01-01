using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using System.Threading;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IGenericRepository<Product> _productsRepo;
    private readonly IGenericRepository<ProductBrand> _productBrandRepo;
    private readonly IGenericRepository<ProductType> _productTypeRepo;

    public ProductsController(IGenericRepository<Product> productsRepo,
        IGenericRepository<ProductBrand> productBrandRepo,
        IGenericRepository<ProductType> productTypeRepo)
    {
        _productsRepo = productsRepo;
        _productBrandRepo = productBrandRepo;
        _productTypeRepo = productTypeRepo;
    }

    #region[GET Requests]

    /// <Summary>
    /// Get The Product List
    /// </Summary>
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetProducts(CancellationToken cancellationToken)
    {
        var products = await this._productsRepo.AllListAsync();

        return Ok(products);
    }

    /// <Summary>
    /// Get The Specific Product
    /// </Summary>
    /// <param name="id"></param>
    /// <returns> Product with specific id </returns>
    [HttpGet("{id}")]
    public async  Task<ActionResult<string>> GetProduct(int id, CancellationToken cancellationToken)
    {
        var product = await this._productsRepo.GetByIdAsync(id);

        return Ok(product);
    }

    /// <Summary>
    /// Get The Product Brands
    /// </Summary>
    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands(CancellationToken cancellationToken)
    {
        var productBrands = await this._productBrandRepo.AllListAsync();

        return Ok(productBrands);
    }

    /// <Summary>
    /// Get The Product Types
    /// </Summary>
    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductTypes(CancellationToken cancellationToken)
    {
        var productTypes = await this._productTypeRepo.AllListAsync();

        return Ok(productTypes);
    }

    #endregion
}
