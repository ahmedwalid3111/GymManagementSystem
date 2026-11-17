using GymManagementSystem.DAL.Data.DbContexts;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        public ISessionRepository SessionRepository { get; }
        private readonly GymDbContext _dbContext;
        private readonly Dictionary<string, object> repositories = [];
         
        public UnitOfWork(GymDbContext dbContext, ISessionRepository sessionRepository)
        {
            _dbContext = dbContext;
            SessionRepository = sessionRepository;

        }

        

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var typeName = typeof(TEntity).Name; 
            if(repositories.TryGetValue(typeName, out var repository))
            {
                return (IGenericRepository<TEntity>)repository;
            }
            var repo = new GenericRepository<TEntity>(_dbContext);
            repositories[typeName] = repo;
            return repo;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _dbContext.SaveChangesAsync(ct);
        }
    }
}
