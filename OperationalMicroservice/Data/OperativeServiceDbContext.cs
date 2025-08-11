using Microsoft.EntityFrameworkCore;
using OperationalMicroservice.Data.Entities;
using System.Collections.Generic;

namespace OperationalMicroservice.Data
{
    public class OperativeDbContext : DbContext
    {
        public OperativeDbContext(DbContextOptions<OperativeDbContext> options) : base(options) { }

        public DbSet<DiceRoll> DiceRolls => Set<DiceRoll>();
    }
}
