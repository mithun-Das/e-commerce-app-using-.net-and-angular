﻿
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly StoreContext _context;

    public GenericRepository(StoreContext context)
    {
        _context = context;
    }

    public async Task<T> GetByIdAsync(int id) 
    {
        var result = await this._context.Set<T>().FindAsync(id);

        return result;
    }

    public async Task<IReadOnlyList<T>> AllListAsync()
    {
        var data = await this._context.Set<T>().ToListAsync();

        return data;
    }

    public async Task<T> GetEntityWithSpec(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).ToListAsync();
    }

    public async Task<int> CountAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).CountAsync();
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        return SpecificationEvaluator<T>.Getquery(this._context.Set<T>().AsQueryable(), spec);
    }

    public void Add(T entity)
    {
        this._context.Set<T>().Add(entity);
    }

    public void Update(T entity)
    {
        this._context.Set<T>().Attach(entity);
        this._context.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity)
    {
        this._context.Set<T>().Remove(entity);
    }
}
