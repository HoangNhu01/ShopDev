using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;
using ShopDev.Constants.Database;
using ShopDev.InfrastructureBase.Persistence;

namespace ShopDev.Chat.Infrastructure.Persistence
{
	public partial class ChatDbContext : ApplicationDbContext
	{

		public DbSet<OpenIddictEntityFrameworkCoreAuthorization> OpenIddictAuthorizations { get; set; }
		public DbSet<OpenIddictEntityFrameworkCoreToken> OpenIddictTokens { get; set; }

		public ChatDbContext()
			: base() { }

		public ChatDbContext(
			DbContextOptions<ChatDbContext> options,
			IHttpContextAccessor httpContextAccessor
		)
			: base(options, httpContextAccessor) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasDefaultSchema(DbSchemas.SDChat);
		}
	}
}
