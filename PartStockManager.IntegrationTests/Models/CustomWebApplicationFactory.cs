using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using PartStockManager.Adapter.Database.Context;

namespace PartStockManager.IntegrationTests.Models
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        // The databse in memory
        private readonly InMemoryDatabaseRoot _dbRoot = new InMemoryDatabaseRoot();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // 1. Clean everything in Entity Framework (internal services and options)
                var efServices = services.Where(d =>
                    d.ServiceType.FullName.Contains("Microsoft.EntityFrameworkCore")).ToList();

                foreach (var service in efServices)
                {
                    services.Remove(service);
                }

                // 2. Recreate the database context
                services.AddDbContext<StockDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemStubDb", _dbRoot);
                });
            });
        }

        public void ResetDatabase()
        {
            using (var scope = Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<StockDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }

        public void SeedData(Action<StockDbContext> seedAction)
        {
            using (var scope = Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<StockDbContext>();
                seedAction(db);
                db.SaveChanges();
            }
        }
    }
}