using SalesPoint.Models;

namespace SalesPoint.Interfaces
{
    public interface IAuditLogRepository
    {
        Task AddAuditLogAsync(AuditLog auditLog);
        Task<IEnumerable<AuditLog>> GetAllAuditLogsAsync(DateTime? fromDate = null, DateTime? toDate = null, string? action = null, string? model = null);
    }
}
