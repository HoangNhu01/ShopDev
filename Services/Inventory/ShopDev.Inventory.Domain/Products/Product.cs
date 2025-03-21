﻿using Microsoft.EntityFrameworkCore;
using ShopDev.Abstractions.EntitiesBase.Interfaces;
using ShopDev.Constants.Database;
using ShopDev.Inventory.Domain.Categories;
using ShopDev.Inventory.Domain.Shops;

namespace ShopDev.Inventory.Domain.Products
{
    [Table(nameof(Product), Schema = DbSchemas.SDInventory)]
    [Index(
        nameof(Name),
        nameof(Title),
        nameof(Deleted),
        AllDescending = true,
        Name = $"IX_{nameof(Product)}",
        IsUnique = false
    )]
    public class Product : IFullAudited
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        //[BsonElement("name")]
        public required string Name { get; set; }

        [MaxLength(2500)]
        //[BsonElement("description")]
        public required string Description { get; set; }

        [MaxLength(255)]
        //[BsonElement("title")]
        public required string Title { get; set; }

        //[BsonElement("thumbUri")]
        [Unicode(false)]
        public required string ThumbUri { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime SetFeaturedDate { get; set; }

        //[BsonElement("price")]
        public double Price { get; set; }

        //[BsonElement("shopId")]
        public int ShopId { get; set; }
        public virtual Shop Shop { get; } = null!;

        public virtual List<Spu> Spus { get; set; } = [];
        #region audit
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public int? DeletedBy { get; set; }
        public bool Deleted { get; set; }
        #endregion
        //[BsonElement("categories")]
        public virtual List<CategoryType> Categories { get; set; } = [];

        //[BsonElement("attributes")]
        public List<AttributeType> Attributes { get; set; } = [];

        //[BsonElement("variations")]
        public List<Variation> Variations { get; set; } = [];
    }

    public class AttributeType
    {
        //[BsonElement("attribute_id")]
        public Guid AttributeId { get; set; }

        //[BsonElement("attribute_name")]
        public required string Name { get; set; }

        //[BsonElement("attribute_value")]
        public required string Value { get; set; }
    }

    public class Variation
    {
        //[BsonElement("variation_name")]
        public required string Name { get; set; }

        //[BsonElement("variation_options")]
        public List<string> Options { get; set; } = [];
    }
}
