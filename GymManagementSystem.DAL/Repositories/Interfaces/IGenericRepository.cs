using GymManagementSystem.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<T>where T:BaseEntity,new()
    {
        Task<IEnumerable<T>> GetAllAsync(bool tracking = false, CancellationToken ct = default);
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
        void AddAsync(T entity);
        void UpdateAsync(T entity);
        void DeleteAsync(T entity);
        Task<bool>AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct= default);
        Task<T?>FirstOrDefaultAsync(Expression<Func<T, bool>> predicate,bool tracking = false,CancellationToken ct = default);
    }
}
