using GymManagementSystem.DAL.Data.DbContexts;
using GymManagementSystem.DAL.Data.Models;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Repositories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext _dbContext;
        public SessionRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(CancellationToken ct = default)
        {
            var sessions = _dbContext.Sessions.Include(S => S.Category).Include(S => S.Trainer);
            return await sessions.ToListAsync(ct);
        }

        public async Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default)
        {
            return await _dbContext.Bookings.CountAsync(B => B.SessionId == sessionId, ct);
        }

        public async Task<Session?> GetSessionWithTrainerAndCategoryAsync(int sessionId, CancellationToken ct = default)
        {
            return await _dbContext.Sessions.AsNoTracking().Include(S => S.Category).Include(S => S.Trainer).FirstOrDefaultAsync(S => S.Id == sessionId, ct);
        }
    }
}
