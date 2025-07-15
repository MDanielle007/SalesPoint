using SalesPoint.Interfaces;
using SalesPoint.Models;
using System.Text.Json;

namespace SalesPoint.Services
{
    public class AuditLogService
    {
        private readonly IAuditLogRepository _auditLogRespository;
        private readonly IHttpContextAccessor _contextAccessor;

        public AuditLogService(
            IAuditLogRepository auditLogRepository,
            IHttpContextAccessor contextAccessor)
        {
            _auditLogRespository = auditLogRepository;
            _contextAccessor = contextAccessor;
        }

        public async Task LogAsync(int userId, string action, string model, int recordId, string description, object? changes = null)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                Model = model,
                RecordId = recordId,
                Description = description,
                Changes = changes != null ? JsonSerializer.Serialize(changes) : null,
                IpAddress = _contextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString(),
                UserAgent = _contextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString()
            };

            await _auditLogRespository.AddAuditLogAsync(log);
        }
    }
}
