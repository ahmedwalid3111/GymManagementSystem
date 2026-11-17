using GymManagementSystem.DAL.Data.DbContexts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.DAL.Data.DataSeed
{
    public static class GymDataSeeding
    {
        public static void SeedAsync(GymDbContext dbContext, string sedFilePath, ILogger logger, CancellationToken ct=default) 
        {
        }
    }
}
