﻿using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class GenericRepository<TEntity, TKey>(StoreDbContext _context) : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        public async Task<IEnumerable<TEntity>> GetAllAsync(bool trackChanges = false)
        {
            if (typeof(TEntity) == typeof(Product))
            {
                return trackChanges ?
                    await _context.Products.Include(p => p.ProductType).Include(p => p.ProductBrand).ToListAsync() as IEnumerable<TEntity> :
                    await _context.Products.Include(p => p.ProductType).Include(p => p.ProductBrand).AsNoTracking().ToListAsync() as IEnumerable<TEntity>;
            }
            return trackChanges ?
                   await _context.Set<TEntity>().ToListAsync() :
                   await _context.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetAsync(TKey id)
        {
            if (typeof(TEntity) == typeof(Product))
            {
                return await _context.Products.Include(p => p.ProductBrand).Include(p => p.ProductType).FirstOrDefaultAsync(p => p.Id == id as int?) as TEntity;
            }
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task AddAsync(TEntity entity) => await _context.AddAsync(entity);

        public void Update(TEntity entity) => _context.Update(entity);

        public void Delete(TEntity entity) => _context.Remove(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecificaitons<TEntity, TKey> spec, bool trackChanges = false)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<TEntity?> GetAsync(ISpecificaitons<TEntity, TKey> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<int> CountAsync(ISpecificaitons<TEntity, TKey> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }
        private IQueryable<TEntity> ApplySpecification(ISpecificaitons<TEntity, TKey> spec)
        {
            return SpecificationEvaluator.GetQuery(_context.Set<TEntity>(), spec);
        }

     
    }
}
