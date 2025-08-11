using OperationalMicroservice.Data.DTOs;
using OperationalMicroservice.Data.Entities;
using OperationalMicroservice.Data;
using Microsoft.EntityFrameworkCore;

namespace OperationalMicroservice.Services.DiceService
{
    public class DiceService : IDiceService
    {
        private readonly OperativeDbContext _db;
        private readonly Random _rng = new Random();

        public DiceService(OperativeDbContext db)
        {
            _db = db;
        }

        public async Task<RollResponseDTO> RollAsync(Guid userId)
        {
            var die1 = _rng.Next(1, 7);
            var die2 = _rng.Next(1, 7);

            var entity = new DiceRoll
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Die1 = die1,
                Die2 = die2,
                // CreatedAt set by default
            };

            _db.DiceRolls.Add(entity);
            await _db.SaveChangesAsync();

            return new RollResponseDTO
            {
                Die1 = die1,
                Die2 = die2,
                Sum = die1 + die2,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<PagedResult<RollResponseDTO>> GetHistoryAsync(Guid userId, HistoryQueryParams query)
        {
            var q = _db.DiceRolls.AsNoTracking().Where(r => r.UserId == userId);

            // Filters
            if (query.Year.HasValue)
                q = q.Where(r => r.CreatedAt.Year == query.Year.Value);

            if (query.Month.HasValue)
                q = q.Where(r => r.CreatedAt.Month == query.Month.Value);

            if (query.Day.HasValue)
                q = q.Where(r => r.CreatedAt.Day == query.Day.Value);

            // Sorting: if both specified, **sum has priority** (per requirement)
            bool hasSortSum = !string.IsNullOrWhiteSpace(query.SortBySum) &&
                              (query.SortBySum.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                               query.SortBySum.Equals("desc", StringComparison.OrdinalIgnoreCase));
            bool hasSortDate = !string.IsNullOrWhiteSpace(query.SortByDate) &&
                              (query.SortByDate.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                               query.SortByDate.Equals("desc", StringComparison.OrdinalIgnoreCase));

            // Apply sorting
            if (hasSortSum && hasSortDate)
            {
                // primary: sum, secondary: date
                q = ApplyOrderBy(q,
                    primarySelector: r => r.Die1 + r.Die2,
                    primaryAsc: query.SortBySum!.Equals("asc", StringComparison.OrdinalIgnoreCase),
                    secondarySelector: r => r.CreatedAt,
                    secondaryAsc: query.SortByDate!.Equals("asc", StringComparison.OrdinalIgnoreCase));
            }
            else if (hasSortSum)
            {
                q = query.SortBySum!.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? q.OrderBy(r => r.Die1 + r.Die2)
                    : q.OrderByDescending(r => r.Die1 + r.Die2);
            }
            else if (hasSortDate)
            {
                q = query.SortByDate!.Equals("asc", StringComparison.OrdinalIgnoreCase)
                    ? q.OrderBy(r => r.CreatedAt)
                    : q.OrderByDescending(r => r.CreatedAt);
            }
            else
            {
                // default: newest first
                q = q.OrderByDescending(r => r.CreatedAt);
            }

            var total = await q.LongCountAsync();

            var page = Math.Max(1, query.Page);
            var pageSize = Math.Clamp(query.PageSize, 1, 100);
            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RollResponseDTO
                {
                    Die1 = r.Die1,
                    Die2 = r.Die2,
                    Sum = r.Die1 + r.Die2,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return new PagedResult<RollResponseDTO>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                Items = items
            };
        }

        // Helper: apply primary and secondary ordering
        private static IQueryable<DiceRoll> ApplyOrderBy<TKey1, TKey2>(
            IQueryable<DiceRoll> query,
            System.Linq.Expressions.Expression<Func<DiceRoll, TKey1>> primarySelector,
            bool primaryAsc,
            System.Linq.Expressions.Expression<Func<DiceRoll, TKey2>> secondarySelector,
            bool secondaryAsc)
        {
            if (primaryAsc)
            {
                return secondaryAsc
                    ? query.OrderBy(primarySelector).ThenBy(secondarySelector)
                    : query.OrderBy(primarySelector).ThenByDescending(secondarySelector);
            }
            else
            {
                return secondaryAsc
                    ? query.OrderByDescending(primarySelector).ThenBy(secondarySelector)
                    : query.OrderByDescending(primarySelector).ThenByDescending(secondarySelector);
            }
        }
    }
}
