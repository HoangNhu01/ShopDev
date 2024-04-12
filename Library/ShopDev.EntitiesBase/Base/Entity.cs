namespace ShopDev.EntitiesBase.Base
{
    public class Entity<TKey> : IEntity<TKey>
    {
        public required TKey Id { get; set; }
    }

    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }

    public class Entity : Entity<int> { }
}
