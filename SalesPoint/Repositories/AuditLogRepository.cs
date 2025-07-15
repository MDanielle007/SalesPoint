using Microsoft.EntityFrameworkCore;
using SalesPoint.Data;
using SalesPoint.Interfaces;
using SalesPoint.Models;

namespace SalesPoint.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAuditLogAsync(AuditLog auditLog)
        {
            await _context.AuditLogs.AddAsync(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAllAuditLogsAsync(DateTime? fromDate = null, DateTime? toDate = null, string? action = null, string? model = null)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.CreatedAt >= fromDate);
            }

            if (toDate.HasValue)
            {
                query = query.Where(a => a.CreatedAt <= toDate);
            }

            if (!string.IsNullOrWhiteSpace(action))
            {
                query = query.Where(a => a.Action == action);
            }

            if (!string.IsNullOrWhiteSpace(model))
            {
                query = query.Where(a => a.Model == model);
            }

            return await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
        }
    }
}
