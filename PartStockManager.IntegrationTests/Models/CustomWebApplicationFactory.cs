using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PartStockManager.Adapter.Database.Context;
using PartStockManager.CoreLogic.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PartStockManager.IntegrationTests.Models
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        // The databse in memory
        private readonly InMemoryDatabaseRoot _dbRoot = new InMemoryDatabaseRoot();
        private const string TestJwtKey = "ThisIsAFakeKeyForTestingPurposesOnly123!";
        private const string TestJwtIssuer = "TestIssuer";
        private const string TestJwtAudience = "TestAudience";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing"); builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Jwt:Key"] = TestJwtKey,       // ← utilise la constante
                    ["Jwt:Issuer"] = TestJwtIssuer, // ← utilise la constante
                    ["Jwt:Audience"] = TestJwtAudience, // ← utilise la constante
                    ["Seed:AdminUsername"] = "admin",
                    ["Seed:AdminPassword"] = "admin"
                }!);
            });

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

        public string GenerateTestToken(UserProfile profile, string username = "testuser")
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, profile.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestJwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: TestJwtIssuer,
                audience: TestJwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}