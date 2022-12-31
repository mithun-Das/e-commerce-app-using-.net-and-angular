using Core.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace Core.Interfaces;

public interface IProductRepository
{
    Task<Product> GetProductByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Product>> GetProductsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductType>> GetProductTypesAsync(CancellationToken cancellationToken);
}
