using GymManagementSystem.DAL.Data.DbContexts;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity, new()
    {
        private readonly GymDbContext _dbContext;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(GymDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            var entities = tracking ? _dbSet.ToListAsync(ct) : _dbSet.AsNoTracking().ToListAsync(ct);
            return await entities;
        }
        public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = _dbSet.FindAsync(id, ct);
            return await entity;
        }
        public async Task<int> AddAsync(T entity, CancellationToken ct = default)
        {
            _dbSet.Add(entity);
            return await _dbContext.SaveChangesAsync(ct);
        }
        public async Task<int> UpdateAsync(T entity, CancellationToken ct = default)
        {
            _dbSet.Update(entity);
            return await _dbContext.SaveChangesAsync(ct);
        }
        public async Task<int> DeleteAsync(T entity, CancellationToken ct = default)
        {
            _dbSet.Remove(entity);
            return await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            

            return await _dbSet.AnyAsync(predicate, ct);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool tracking = false, CancellationToken ct = default)
        {
            var result = tracking ? _dbSet.FirstOrDefaultAsync(predicate, ct) : _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, ct);
            return await result;
        }
    }
}
