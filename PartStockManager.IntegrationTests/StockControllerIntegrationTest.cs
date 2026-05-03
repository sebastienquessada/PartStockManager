using PartStockManager.Adapter.Database.Entities;
using PartStockManager.Adapter.Models;
using PartStockManager.IntegrationTests.Models;
using System.Net;
using System.Net.Http.Json;

namespace PartStockManager.IntegrationTests
{
    public class StockControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public StockControllerIntegrationTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;

            _factory.ResetDatabase();

            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Inventory_Success()
        {
            var expected = HttpStatusCode.OK;

            _factory.SeedData(db =>
            {
                var entity = new PartEntity
                {
                    Reference = "PART001",
                    Name = "My First Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART002",
                    Name = "My Second Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART003",
                    Name = "My Third Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var inventoryItems = new List<InventoryItem>()
            {
                new InventoryItem("PART001", 10),
                new InventoryItem("PART002", 5),
                new InventoryItem("PART003", 8)
            };

            var request = new InventoryRequest(inventoryItems);

            var response = await _client.PostAsJsonAsync("/api/Stock/inventory", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Inventory_Failed_Reference_Not_Found()
        {
            var expected = HttpStatusCode.NotFound;

            _factory.SeedData(db =>
            {
                var entity = new PartEntity
                {
                    Reference = "PART001",
                    Name = "My First Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART002",
                    Name = "My Second Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART003",
                    Name = "My Third Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var inventoryItems = new List<InventoryItem>()
            {
                new InventoryItem("PART001", 10),
                new InventoryItem("PART008", 5),
                new InventoryItem("PART003", 8)
            };

            var request = new InventoryRequest(inventoryItems);

            var response = await _client.PostAsJsonAsync("/api/Stock/inventory", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Inventory_Failed_Negative_Quantity()
        {
            var expected = HttpStatusCode.BadRequest;

            _factory.SeedData(db =>
            {
                var entity = new PartEntity
                {
                    Reference = "PART001",
                    Name = "My First Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART002",
                    Name = "My Second Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART003",
                    Name = "My Third Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var inventoryItems = new List<InventoryItem>()
            {
                new InventoryItem("PART001", 10),
                new InventoryItem("PART002", 5),
                new InventoryItem("PART003", -8)
            };

            var request = new InventoryRequest(inventoryItems);

            var response = await _client.PostAsJsonAsync("/api/Stock/inventory", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Stock_Exit_Record_Success()
        {
            var expected = HttpStatusCode.OK;

            _factory.SeedData(db =>
            {
                var entity = new PartEntity
                {
                    Reference = "PART001",
                    Name = "My First Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART002",
                    Name = "My Second Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART003",
                    Name = "My Third Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var request = new StockMovementRequest();

            request.Reference = "PART002";
            request.Quantity = 1;

            var response = await _client.PostAsJsonAsync("/api/Stock/exit", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Stock_Exit_Record_Failed_Reference_Not_Found()
        {
            var expected = HttpStatusCode.NotFound;

            _factory.SeedData(db =>
            {
                var entity = new PartEntity
                {
                    Reference = "PART001",
                    Name = "My First Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART002",
                    Name = "My Second Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART003",
                    Name = "My Third Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var request = new StockMovementRequest();

            request.Reference = "PART0025";
            request.Quantity = 1;

            var response = await _client.PostAsJsonAsync("/api/Stock/exit", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Stock_Exit_Record_Failed_Empty_Reference()
        {
            var expected = HttpStatusCode.BadRequest;

            _factory.SeedData(db =>
            {
                var entity = new PartEntity
                {
                    Reference = "PART001",
                    Name = "My First Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART002",
                    Name = "My Second Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART003",
                    Name = "My Third Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var request = new StockMovementRequest();

            request.Reference = " ";
            request.Quantity = 1;

            var response = await _client.PostAsJsonAsync("/api/Stock/exit", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Stock_Exit_Record_Failed_Negative_Quantity()
        {
            var expected = HttpStatusCode.BadRequest;

            _factory.SeedData(db =>
            {
                var entity = new PartEntity
                {
                    Reference = "PART001",
                    Name = "My First Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART002",
                    Name = "My Second Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART003",
                    Name = "My Third Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var request = new StockMovementRequest();

            request.Reference = "PART002";
            request.Quantity = -1;

            var response = await _client.PostAsJsonAsync("/api/Stock/exit", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Stock_Exit_Record_Failed_Available_Quantity_Too_Low()
        {
            var expected = HttpStatusCode.BadRequest;

            _factory.SeedData(db =>
            {
                var entity = new PartEntity
                {
                    Reference = "PART001",
                    Name = "My First Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART002",
                    Name = "My Second Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART003",
                    Name = "My Third Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var request = new StockMovementRequest();

            request.Reference = "PART002";
            request.Quantity = 20;

            var response = await _client.PostAsJsonAsync("/api/Stock/exit", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Stock_Entry_Record_Success()
        {
            var expected = HttpStatusCode.OK;

            _factory.SeedData(db =>
            {
                var entity = new PartEntity
                {
                    Reference = "PART001",
                    Name = "My First Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART002",
                    Name = "My Second Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART003",
                    Name = "My Third Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var request = new StockMovementRequest();

            request.Reference = "PART002";
            request.Quantity = 5;

            var response = await _client.PostAsJsonAsync("/api/Stock/entry", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Stock_Entry_Record_Failed_Reference_Not_Found()
        {
            var expected = HttpStatusCode.NotFound;

            _factory.SeedData(db =>
            {
                var entity = new PartEntity
                {
                    Reference = "PART001",
                    Name = "My First Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART002",
                    Name = "My Second Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART003",
                    Name = "My Third Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var request = new StockMovementRequest();

            request.Reference = "REF-NOT-FOUND";
            request.Quantity = 1;

            var response = await _client.PostAsJsonAsync("/api/Stock/exit", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Stock_Entry_Record_Failed_Empty_Reference()
        {
            var expected = HttpStatusCode.BadRequest;

            _factory.SeedData(db =>
            {
                var entity = new PartEntity
                {
                    Reference = "PART001",
                    Name = "My First Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART002",
                    Name = "My Second Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART003",
                    Name = "My Third Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var request = new StockMovementRequest();

            request.Reference = " ";
            request.Quantity = 1;

            var response = await _client.PostAsJsonAsync("/api/Stock/entry", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Stock_Entry_Record_Failed_Negative_Quantity()
        {
            var expected = HttpStatusCode.BadRequest;

            _factory.SeedData(db =>
            {
                var entity = new PartEntity
                {
                    Reference = "PART001",
                    Name = "My First Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART002",
                    Name = "My Second Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);

                entity = new PartEntity
                {
                    Reference = "PART003",
                    Name = "My Third Part",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var request = new StockMovementRequest();

            request.Reference = "PART002";
            request.Quantity = -1;

            var response = await _client.PostAsJsonAsync("/api/Stock/entry", request);

            Assert.Equal(expected, response.StatusCode);
        }
    }
}
