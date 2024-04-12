namespace ShopDev.Abstractions.EntitiesBase.Interfaces
{
    public interface IFullAudited : ICreatedBy, IModifiedBy, ISoftDelted { }

    public interface ISoftDelted
    {
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public int? DeletedBy { get; set; }
    }

    public interface ICreatedBy
    {
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
    }

    public interface IModifiedBy
    {
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
    }
}
