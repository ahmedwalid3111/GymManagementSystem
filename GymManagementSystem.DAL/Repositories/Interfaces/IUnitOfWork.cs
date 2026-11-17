using GymManagementSystem.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity,new();
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        public ISessionRepository SessionRepository { get; }
    }
}
