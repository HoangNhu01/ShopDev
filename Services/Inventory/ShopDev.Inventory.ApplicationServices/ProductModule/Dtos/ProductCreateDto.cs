﻿using MongoDB.Bson;
using ShopDev.ApplicationBase.Common.Validations;

namespace ShopDev.Inventory.ApplicationServices.ProductModule.Dtos
{
    public class ProductCreateDto
    {
        [CustomMaxLength(100)]
        public required string Name { get; set; }
        [CustomMaxLength(2500)]
        public required string Description { get; set; }

        [CustomMaxLength(255)]
        public required string Title { get; set; }
        public required string ThumbUri { get; set; }
        public double Price { get; set; }
        public int ShopId { get; set; }
        public List<AttributeCreateDto> Attributes { get; set; } = [];
        public List<CategoryTypeDto> Categories { get; set; } = [];
        public List<VariationCreateDto> Variations { get; set; } = [];
        public List<SpuCreateDto> Spus { get; set; } = [];
    }
    public class CategoryTypeDto
    {
        public int CategoryId { get; set; }
    }
    public class AttributeCreateDto
    {
        [CustomMaxLength(255)]
        public required string Name { get; set; }

        [CustomMaxLength(255)]
        public required string Value { get; set; }
    }

    public class VariationCreateDto
    {
        [CustomMaxLength(255)]
        public required string Name { get; set; }

        [CustomMaxLength(255)]
        public List<string> Options { get; set; } = [];
    }

    public class SpuCreateDto
    {
        public List<int> Index { get; set; } = [];
        public double Price { get; set; }
        public int Stock { get; set; }
    }
}
