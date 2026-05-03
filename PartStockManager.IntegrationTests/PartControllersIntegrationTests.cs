
using PartStockManager.Adapter.Database.Entities;
using PartStockManager.Adapter.Models;
using PartStockManager.CoreLogic.Models;
using PartStockManager.IntegrationTests.Models;
using System.Net;
using System.Net.Http.Json;

namespace PartStockManager.IntegrationTests
{
    public class PartControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public PartControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            
            _factory.ResetDatabase();

            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Create_Part_Success()
        {
            var expected = HttpStatusCode.OK;

            var request = new PartDto { Reference = "TEST-CREATION-SUCCEED", Name = "Test", StockQuantity = 5, LowStockThreshold = 2 };

            var response = await _client.PostAsJsonAsync("/api/Part/create", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Create_Part_Fail_Empty_Name()
        {
            var expected = HttpStatusCode.BadRequest;

            var request = new PartDto { Reference = "FAILED-EMPTY-NAME", Name = "", StockQuantity = 5, LowStockThreshold = 2 };

            var response = await _client.PostAsJsonAsync("/api/Part/create", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Create_Part_Fail_Empty_Reference()
        {
            var expected = HttpStatusCode.BadRequest;

            var request = new PartDto { Reference = "", Name = "FAILED-EMPTY-REF", StockQuantity = 5, LowStockThreshold = 2 };

            var response = await _client.PostAsJsonAsync("/api/Part/create", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Create_Part_Fail_Reference_Already_Exists()
        {
            var expected = HttpStatusCode.Conflict;

            string refTest = "FAILED-ALREADY-EXIST";
            _factory.SeedData(db => {
                var entity = new PartEntity
                {
                    Reference = refTest,
                    Name = "Vieux Nom",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var request = new PartDto { Reference = refTest, Name = "FAILED Ref Already Exist", StockQuantity = 5, LowStockThreshold = 2 };

            var response = await _client.PostAsJsonAsync("/api/Part/create", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Create_Part_Fail_Negative_Quantity()
        {
            var expected = HttpStatusCode.BadRequest;

            var request = new PartDto { Reference = "FAILED-NEGATIVE-QUANTITY", Name = "My Part", StockQuantity = -5, LowStockThreshold = 2 };

            var response = await _client.PostAsJsonAsync("/api/Part/create", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Create_Part_Fail_Negative_LowStockThreshold()
        {
            var expected = HttpStatusCode.BadRequest;

            var request = new PartDto { Reference = "FAILED-NEGATIVE-QUANTITY", Name = "My Part", StockQuantity = 5, LowStockThreshold = -2 };

            var response = await _client.PostAsJsonAsync("/api/Part/create", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Modify_Part_Success()
        {
            var expected = HttpStatusCode.OK;

            string refTest = "SUCCEEDED-MODIF";
            _factory.SeedData(db => {
                var entity = new PartEntity
                {
                    Reference = refTest,
                    Name = "Vieux Nom",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var request = new PartModificationRequest
            {
                CurrentPartReference = refTest,
                UpdatedPart = new PartDto
                {
                    Reference = refTest,
                    Name = "Nouveau Nom",
                    StockQuantity = 5,
                    LowStockThreshold = 2
                }
            };

            var response = await _client.PutAsJsonAsync("/api/Part/modify", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Modify_Part_Fail_Negative_Quantity()
        {
            var expected = HttpStatusCode.BadRequest;

            string refTest = "NEGATIVE-QUANTITY";
            _factory.SeedData(db => {
                var entity = new PartEntity
                {
                    Reference = refTest,
                    Name = "Vieux Nom",
                    StockQuantity = 10,
                    LowStockThreshold = 3
                };

                db.Parts.Add(entity);
            });

            var request = new PartModificationRequest
            {
                CurrentPartReference = refTest,
                UpdatedPart = new PartDto
                {
                    Reference = refTest,
                    Name = "Nouveau Nom",
                    StockQuantity = -10,
                    LowStockThreshold = 2
                }
            };

            var response = await _client.PutAsJsonAsync("/api/Part/modify", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Modify_Part_Fail_Negative_LowStockThreshold()
        {
            var expected = HttpStatusCode.BadRequest;

            string refTest = "FAILED-NEG-THRESHOLD";
            _factory.SeedData(db => {
                var entity = new PartEntity
                {
                    Reference = refTest,
                    Name = "Vieux Nom",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var request = new PartModificationRequest
            {
                CurrentPartReference = refTest,
                UpdatedPart = new PartDto
                {
                    Reference = refTest,
                    Name = "Nouveau Nom",
                    StockQuantity = 10,
                    LowStockThreshold = -2
                }
            };

            var response = await _client.PutAsJsonAsync("/api/Part/modify", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Modify_Part_Fail_Empty_Reference()
        {
            var expected = HttpStatusCode.BadRequest;

            string refTest = "FAIL-EMPTY-REF";
            _factory.SeedData(db => {
                var entity = new PartEntity
                {
                    Reference = refTest,
                    Name = "Vieux Nom",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var request = new PartModificationRequest
            {
                CurrentPartReference = refTest,
                UpdatedPart = new PartDto
                {
                    Reference = " ",
                    Name = "Nouveau Nom",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                }
            };

            var response = await _client.PutAsJsonAsync("/api/Part/modify", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Modify_Part_Fail_Empty_Name()
        {
            var expected = HttpStatusCode.BadRequest;

            string refTest = "FAIL-EMPTY-REF";
            _factory.SeedData(db => {
                var entity = new PartEntity
                {
                    Reference = refTest,
                    Name = "Vieux Nom",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            var request = new PartModificationRequest
            {
                CurrentPartReference = refTest,
                UpdatedPart = new PartDto
                {
                    Reference = refTest,
                    Name = " ",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                }
            };

            var response = await _client.PutAsJsonAsync("/api/Part/modify", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Part_Successed()
        {
            var expected = HttpStatusCode.OK;

            string refTest = "DELETION-OK";
            _factory.SeedData(db => {
                var entity = new PartEntity
                {
                    Reference = refTest,
                    Name = "Vieux Nom",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            PartDeletionRequest request = new PartDeletionRequest
            {
                PartReference = refTest
            };



            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "/api/Part/delete")
            {
                Content = JsonContent.Create(request)
            };

            var response = await _client.SendAsync(httpRequest);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Delete_Part_Fail_Empty_Reference()
        {
            var expected = HttpStatusCode.BadRequest;

            _factory.SeedData(db => {
                var entity = new PartEntity
                {
                    Reference = "FAILED-DELETION",
                    Name = "Vieux Nom",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            PartDeletionRequest request = new PartDeletionRequest
            {
                PartReference = " "
            };



            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "/api/Part/delete")
            {
                Content = JsonContent.Create(request)
            };

            var response = await _client.SendAsync(httpRequest);

            Assert.Equal(expected, response.StatusCode);
        }



        [Fact]
        public async Task Delete_Part_Fail_No_Part()
        {
            var expected = HttpStatusCode.NotFound;

            _factory.SeedData(db => {
                var entity = new PartEntity
                {
                    Reference = "FAILED-DELETION",
                    Name = "Vieux Nom",
                    StockQuantity = 10,
                    LowStockThreshold = 2
                };

                db.Parts.Add(entity);
            });

            PartDeletionRequest request = new PartDeletionRequest
            {
                PartReference = "NOT-EXISTING-PART"
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "/api/Part/delete")
            {
                Content = JsonContent.Create(request)
            };

            var response = await _client.SendAsync(httpRequest);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Get_Parts_With_Name()
        {
            var expected = new List<Part>() {
                new Part("My Second Part", "PART002", 10, 2)
            };

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

            var name = "my second";

            var url = $"/api/Part/get/Parts?Name={name}";

            var response = await _client.GetAsync(url);
            var parts = await response.Content.ReadFromJsonAsync<List<Part>>();

            Assert.Equal(expected, parts);
        }

        [Fact]
        public async Task Get_Parts_With_Name_And_Reference()
        {
            var expected = new List<Part>() {
                new Part("My First Part", "PART001", 10, 2)
            };

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

            var name = "first";
            var reference = "00";

            var url = $"/api/Part/get/Parts?Name={name}&Reference={reference}";

            var response = await _client.GetAsync(url);
            var parts = await response.Content.ReadFromJsonAsync<List<Part>>();

            Assert.Equal(expected, parts);
        }

        [Fact]
        public async Task Get_Parts_With_Reference()
        {
            var expected = new List<Part>() {
                new Part("My First Part", "PART001", 10, 2),
                new Part("My Second Part", "PART002", 10, 2),
                new Part("My Third Part", "PART003", 10, 2)
            };

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

            var reference = "00";

            var url = $"/api/Part/get/Parts?Reference={reference}";

            var response = await _client.GetAsync(url);
            var parts = await response.Content.ReadFromJsonAsync<List<Part>>();

            Assert.Equal(expected, parts);
        }

        [Fact]
        public async Task Get_Parts_With_Empty_Result()
        {
            var expected = new List<Part>() {
            };

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

            var reference = "005";

            var url = $"/api/Part/get/Parts?Reference={reference}";

            var response = await _client.GetAsync(url);
            var parts = await response.Content.ReadFromJsonAsync<List<Part>>();

            Assert.Equal(expected, parts);
        }

        [Fact]
        public async Task Get_Parts_With_Reached_Threshold_Success()
        {
            var expected = new List<Part>() {
                new Part("My Second Part", "PART002", 1, 2)
            };

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
                    StockQuantity = 1,
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

            var url = "/api/Part/get/reached-threshold";

            var response = await _client.GetAsync(url);
            var parts = await response.Content.ReadFromJsonAsync<List<Part>>();

            Assert.Equal(expected, parts);
        }

        [Fact]
        public async Task Get_Parts_With_Reached_Threshold_Success_Empty_Result()
        {
            var expected = new List<Part>() {

            };

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
                    StockQuantity = 3,
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

            var url = "/api/Part/get/reached-threshold";

            var response = await _client.GetAsync(url);
            var parts = await response.Content.ReadFromJsonAsync<List<Part>>();

            Assert.Equal(expected, parts);
        }
    }
}
