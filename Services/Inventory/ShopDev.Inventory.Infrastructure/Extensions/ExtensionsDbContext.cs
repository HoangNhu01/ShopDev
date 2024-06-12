using System.Reflection;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using ShopDev.Abstractions.EntitiesBase.Interfaces;

namespace ShopDev.Inventory.Infrastructure.Extensions
{
    public class ExtensionsDbContext 
    {
        private readonly IMongoDatabase _database;

        public ExtensionsDbContext(IConfiguration config)
        {
            MongoClient client = new(config.GetConnectionString("MongoDb"));
            _database = client.GetDatabase("InventoryDB");
        }

        public IClientSessionHandle GetClientSession()
        {
            return _database.Client.StartSession();
        }

        public IMongoCollection<TEntity> GetMongoCollection<TEntity>()
        {
            string collectionName = typeof(TEntity).Name.Pluralize();
            return _database.GetCollection<TEntity>(collectionName);
        }

        public async Task CreateNewCollectionAsync()
        {
            string assemblyPath =
                $"{Directory.GetCurrentDirectory()}/bin/Debug/net8.0/ShopDev.Inventory.Domain.dll";

            // Tải assembly
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            // Lấy tất cả các loại (types) trong assembly
            Type[] types = assembly
                .ExportedTypes.Where(x => x.BaseType == typeof(IFullAudited))
                .ToArray();

            List<Task> tasks = [];
            // Kiểm tra xem collection đã tồn tại hay chưa
            foreach (var type in types)
            {
                tasks.Add(
                    Task.Run(async () =>
                    {
                        string collectionName = type.Name.Pluralize();
                        var collections = await _database.ListCollectionsAsync(
                            options: new() { Filter = new BsonDocument("name", collectionName) }
                        );
                        if (await collections.AnyAsync())
                        {
                            return; // Collection đã tồn tại
                        }
                        await _database.CreateCollectionAsync(collectionName);
                    })
                );
            }
            await Task.WhenAll(tasks);
        }
    }
}
