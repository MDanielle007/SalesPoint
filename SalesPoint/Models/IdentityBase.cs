using Microsoft.AspNetCore.Identity;

namespace SalesPoint.Models
{
    public abstract class IdentityBase : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted => DeletedAt.HasValue;
    }
}
