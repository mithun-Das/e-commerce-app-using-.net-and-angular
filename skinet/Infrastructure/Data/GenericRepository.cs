
using Core.Entities;
using Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Data;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    public Task<T> GetByIdAsync(int id) 
    {
        throw new System.NotImplementedException();
    }

    public Task<IReadOnlyList<T>> AllListAsync()
    {
        throw new System.NotImplementedException();
    }


}
