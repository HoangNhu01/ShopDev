using Microsoft.EntityFrameworkCore;
using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.Constants.Database;
using ShopDev.Inventory.Domain.Products;

namespace ShopDev.Inventory.Domain.Comments
{
    [Table(nameof(Comment), Schema = DbSchemas.SDInventory)]
    [Index(
        nameof(UserId),
        nameof(ParentId),
        nameof(Deleted),
        AllDescending = true,
        Name = $"IX_{nameof(Comment)}",
        IsUnique = false
    )]
    public class Comment : ISoftDelted
    {
        [Key]
        public Guid Id { get; set; }

        public int Star { get; set; }

        public int UserId { get; set; }
        [MaxLength(255)]
        public required string Content { get; set; }

        public Guid ParentId { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; } = null!;
        public List<MediaComment> MediaComments { get; set; } = [];
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
    }

    public class MediaComment
    {
        public string? Uri { get; set; }

        public string? S3Key { get; set; }
    }
}
