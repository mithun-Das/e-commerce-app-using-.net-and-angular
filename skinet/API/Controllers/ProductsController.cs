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
    public async Task<ActionResult<List<Product>>> GetProducts(CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetProductsAsync(cancellationToken);

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
        var product = await _productRepository.GetProductByIdAsync(id, cancellationToken);

        return Ok(product);
    }

    /// <Summary>
    /// Get The Product Brands
    /// </Summary>
    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands(CancellationToken cancellationToken)
    {
        var productBrands = await _productRepository.GetProductBrandsAsync(cancellationToken);

        return Ok(productBrands);
    }

    /// <Summary>
    /// Get The Product Types
    /// </Summary>
    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductTypes(CancellationToken cancellationToken)
    {
        var productTypes = await _productRepository.GetProductTypesAsync(cancellationToken);

        return Ok(productTypes);
    }

    #endregion
}
