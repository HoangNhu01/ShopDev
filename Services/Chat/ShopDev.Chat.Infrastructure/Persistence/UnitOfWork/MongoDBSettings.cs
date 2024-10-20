namespace ShopDev.Chat.Infrastructure.Persistence.UnitOfWork
{
    public class MongoDBSettings
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
    }
}
