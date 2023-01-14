﻿using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;
using System.Threading;
using Core.Specifications;
using API.Dtos;
using System.Linq;
using AutoMapper;
using API.Errors;
using Microsoft.AspNetCore.Http;

namespace API.Controllers;

public class ProductsController : BaseApiController
{
    private readonly IGenericRepository<Product> _productsRepo;
    private readonly IGenericRepository<ProductBrand> _productBrandRepo;
    private readonly IGenericRepository<ProductType> _productTypeRepo;
    private readonly IMapper _mapper;

    public ProductsController(IGenericRepository<Product> productsRepo,
        IGenericRepository<ProductBrand> productBrandRepo,
        IGenericRepository<ProductType> productTypeRepo,
        IMapper mapper)
    {
        _productsRepo = productsRepo;
        _productBrandRepo = productBrandRepo;
        _productTypeRepo = productTypeRepo;
        _mapper = mapper;
    }

    #region[GET Requests]

    /// <Summary>
    /// Get The Product List
    /// </Summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts(CancellationToken cancellationToken, 
        string? sort, int? brandId, int? typeId)
    {
        var spec = new ProductsWithTypesAndBrandsSpecification(sort, brandId, typeId);

        var products = await this._productsRepo.ListAsync(spec);

        return Ok(this._mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));
    }

    /// <Summary>
    /// Get The Specific Product
    /// </Summary>
    /// <param name="id"></param>
    /// <returns> Product with specific id </returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async  Task<ActionResult<ProductToReturnDto>> GetProduct(int id, CancellationToken cancellationToken)
    {
        var spec = new ProductsWithTypesAndBrandsSpecification(id);

        var product = await this._productsRepo.GetEntityWithSpec(spec);

        if (product == null) return NotFound(new ApiResponse(400));

        return this._mapper.Map<Product, ProductToReturnDto>(product);
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
    public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes(CancellationToken cancellationToken)
    {
        var productTypes = await this._productTypeRepo.AllListAsync();

        return Ok(productTypes);
    }

    #endregion
}
