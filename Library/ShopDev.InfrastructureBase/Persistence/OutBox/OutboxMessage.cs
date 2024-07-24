using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopDev.InfrastructureBase.Persistence.OutBox
{
    [Table(nameof(OutboxMessage))]
    [Index(nameof(ProcessedOnUtc), Name = $"IX_{nameof(OutboxMessage)}")]
    public class OutboxMessage
    {
        [Key]
        public Guid Id { get; set; }
        public string Event { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime OccurredOnUtc { get; set; }
        public DateTime? ProcessedOnUtc { get; set; }
        public string? Error { get; set; }
    }
}
