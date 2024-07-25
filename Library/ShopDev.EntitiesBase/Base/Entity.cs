namespace ShopDev.EntitiesBase.Base
{
    public class Entity<TKey> : IEntity<TKey>, IEquatable<Entity<TKey>>
    {
        public required TKey Id { get; set; }

        public static bool operator ==(Entity<TKey>? first, Entity<TKey>? second)
        {
            return first is not null && second is not null && first.Equals(second);
        }

        public static bool operator !=(Entity<TKey>? first, Entity<TKey>? second) =>
            !(first == second);

        public bool Equals(Entity<TKey>? other)
        {
            if (other is null || other.Id is null)
            {
                return false;
            }

            if (other.GetType() != GetType())
            {
                return false;
            }

            return other.Id.Equals(Id);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            if (obj is not Entity entity)
            {
                return false;
            }

            return entity.Id.Equals(Id);
        }

        public override int GetHashCode() => Id!.GetHashCode() * 41;
    }

    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }

    public class Entity : Entity<int> { }
}
