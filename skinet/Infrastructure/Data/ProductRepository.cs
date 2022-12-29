using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data;

public class ProductRepository : IProductRepository
{
    public Task<Product> GetProductByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Product>> GetProductsAsync()
    {
        throw new NotImplementedException();
    }
}
