using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Data;

public class ProductRepository : IProductRepository
{
    private readonly StoreContext _context;

    public ProductRepository(StoreContext context)
    {
        _context = context;
    }

    public async Task<Product> GetProductByIdAsync(int id, CancellationToken cancellationToken)
    {
        var product = await _context.Products
                        .Include(p => p.ProductBrand)
                        .Include(p => p.ProductType)
                        .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        return product;
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken cancellationToken)
    {
        var products = await _context.Products
                            .Include(p => p.ProductBrand)
                            .Include(p => p.ProductType)
                            .ToListAsync(cancellationToken);

        return products;
    }

    public async Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync(CancellationToken cancellationToken)
    {
        var productBrands = await _context.ProductBrands.ToListAsync(cancellationToken);

        return productBrands;
    }

    public async Task<IReadOnlyList<ProductType>> GetProductTypesAsync(CancellationToken cancellationToken)
    {
        var productTypes = await _context.ProductTypes.ToListAsync();

        return productTypes;
    }
}
