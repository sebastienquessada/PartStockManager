using PartStockManager.API.DTOs;
using PartStockManager.CoreLogic.Models;
using PartStockManager.IntegrationTests.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PartStockManager.IntegrationTests
{
    public class UserControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public UserControllerIntegrationTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;

            _factory.ResetDatabase();

            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Create_User_Success_Admin_Role()
        {
            var token = _factory.GenerateTestToken(UserProfile.Administrator);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.OK;

            var request = new UserDto
            {
                Username = "newuser",
                Password = "Password123!",
                Profile = UserProfile.Manager
            };

            var response = await _client.PostAsJsonAsync("/api/User/create", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Create_User_Fail_Manager_Role()
        {
            var token = _factory.GenerateTestToken(UserProfile.Manager);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.Forbidden;

            var request = new UserDto
            {
                Username = "newuser",
                Password = "Password123!",
                Profile = UserProfile.Manager
            };

            var response = await _client.PostAsJsonAsync("/api/User/create", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Create_User_Fail_Stocktaker_Role()
        {
            var token = _factory.GenerateTestToken(UserProfile.Stocktaker);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.Forbidden;

            var request = new UserDto
            {
                Username = "newuser",
                Password = "Password123!",
                Profile = UserProfile.Manager
            };

            var response = await _client.PostAsJsonAsync("/api/User/create", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Create_User_Fail_No_Token()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var expected = HttpStatusCode.Unauthorized;

            var request = new UserDto
            {
                Username = "newuser",
                Password = "Password123!",
                Profile = UserProfile.Manager
            };

            var response = await _client.PostAsJsonAsync("/api/User/create", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Create_User_Fail_Username_Already_Exists()
        {
            var token = _factory.GenerateTestToken(UserProfile.Administrator);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.Conflict;

            var request = new UserDto
            {
                Username = "duplicateuser",
                Password = "Password123!",
                Profile = UserProfile.Manager
            };

            // First creation should succeed
            await _client.PostAsJsonAsync("/api/User/create", request);

            // Second creation with the same username (should fail)
            var response = await _client.PostAsJsonAsync("/api/User/create", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Create_User_Fail_Empty_Username()
        {
            var token = _factory.GenerateTestToken(UserProfile.Administrator);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.BadRequest;

            var request = new UserDto
            {
                Username = "",
                Password = "Password123!",
                Profile = UserProfile.Manager
            };

            var response = await _client.PostAsJsonAsync("/api/User/create", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Create_User_Fail_Empty_Password()
        {
            var token = _factory.GenerateTestToken(UserProfile.Administrator);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.BadRequest;

            var request = new UserDto
            {
                Username = "newuser",
                Password = "",
                Profile = UserProfile.Manager
            };

            var response = await _client.PostAsJsonAsync("/api/User/create", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Modify_User_Success_Admin_Role()
        {
            var token = _factory.GenerateTestToken(UserProfile.Administrator);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.OK;

            // Create a user to modify
            await _client.PostAsJsonAsync("/api/User/create", new UserDto
            {
                Username = "usertomodify",
                Password = "Password123!",
                Profile = UserProfile.Stocktaker
            });

            var request = new UserModificationRequest
            {
                CurrentUsername = "usertomodify",
                UpdatedUser = new UserDto
                {
                    Username = "usertomodify",
                    Password = "NewPassword123!",
                    Profile = UserProfile.Manager
                }
            };

            var response = await _client.PutAsJsonAsync("/api/User/modify", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Modify_User_Fail_Manager_Role()
        {
            var token = _factory.GenerateTestToken(UserProfile.Manager);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.Forbidden;

            var request = new UserModificationRequest
            {
                CurrentUsername = "someuser",
                UpdatedUser = new UserDto
                {
                    Username = "someuser",
                    Password = "Password123!",
                    Profile = UserProfile.Manager
                }
            };

            var response = await _client.PutAsJsonAsync("/api/User/modify", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Modify_User_Fail_Stocktaker_Role()
        {
            var token = _factory.GenerateTestToken(UserProfile.Stocktaker);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.Forbidden;

            var request = new UserModificationRequest
            {
                CurrentUsername = "someuser",
                UpdatedUser = new UserDto
                {
                    Username = "someuser",
                    Password = "Password123!",
                    Profile = UserProfile.Manager
                }
            };

            var response = await _client.PutAsJsonAsync("/api/User/modify", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Modify_User_Fail_No_Token()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var expected = HttpStatusCode.Unauthorized;

            var request = new UserModificationRequest
            {
                CurrentUsername = "someuser",
                UpdatedUser = new UserDto
                {
                    Username = "someuser",
                    Password = "Password123!",
                    Profile = UserProfile.Manager
                }
            };

            var response = await _client.PutAsJsonAsync("/api/User/modify", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Modify_User_Fail_Username_Not_Found()
        {
            var token = _factory.GenerateTestToken(UserProfile.Administrator);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.NotFound;

            var request = new UserModificationRequest
            {
                CurrentUsername = "doesnotexist",
                UpdatedUser = new UserDto
                {
                    Username = "doesnotexist",
                    Password = "Password123!",
                    Profile = UserProfile.Manager
                }
            };

            var response = await _client.PutAsJsonAsync("/api/User/modify", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Modify_User_Fail_Admin_Modifies_Own_Rights()
        {
            // The token is generated with username "admin" matching CurrentUsername below
            var token = _factory.GenerateTestToken(UserProfile.Administrator, "admin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.BadRequest;

            // Create the "admin" user matching the token's identity
            var adminCreationToken = _factory.GenerateTestToken(UserProfile.Administrator, "rootadmin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminCreationToken);

            await _client.PostAsJsonAsync("/api/User/create", new UserDto
            {
                Username = "admin",
                Password = "Password123!",
                Profile = UserProfile.Administrator
            });

            // Switch back to the "admin" token to attempt to modify its own rights
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var request = new UserModificationRequest
            {
                CurrentUsername = "admin",
                UpdatedUser = new UserDto
                {
                    Username = "admin",
                    Password = "Password123!",
                    Profile = UserProfile.Manager // Trying to downgrade its own profile
                }
            };

            var response = await _client.PutAsJsonAsync("/api/User/modify", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Delete_User_Success_Admin_Role()
        {
            var token = _factory.GenerateTestToken(UserProfile.Administrator);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.OK;

            // Create a user to delete
            await _client.PostAsJsonAsync("/api/User/create", new UserDto
            {
                Username = "usertodelete",
                Password = "Password123!",
                Profile = UserProfile.Stocktaker
            });

            var request = new UserDeletionRequest
            {
                Username = "usertodelete"
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "/api/User/delete")
            {
                Content = JsonContent.Create(request)
            };

            var response = await _client.SendAsync(httpRequest);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Delete_User_Fail_Manager_Role()
        {
            var token = _factory.GenerateTestToken(UserProfile.Manager);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.Forbidden;

            var request = new UserDeletionRequest
            {
                Username = "someuser"
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "/api/User/delete")
            {
                Content = JsonContent.Create(request)
            };

            var response = await _client.SendAsync(httpRequest);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Delete_User_Fail_Stocktaker_Role()
        {
            var token = _factory.GenerateTestToken(UserProfile.Stocktaker);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.Forbidden;

            var request = new UserDeletionRequest
            {
                Username = "someuser"
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "/api/User/delete")
            {
                Content = JsonContent.Create(request)
            };

            var response = await _client.SendAsync(httpRequest);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Delete_User_Fail_No_Token()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var expected = HttpStatusCode.Unauthorized;

            var request = new UserDeletionRequest
            {
                Username = "someuser"
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "/api/User/delete")
            {
                Content = JsonContent.Create(request)
            };

            var response = await _client.SendAsync(httpRequest);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Delete_User_Fail_Username_Not_Found()
        {
            var token = _factory.GenerateTestToken(UserProfile.Administrator);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.NotFound;

            var request = new UserDeletionRequest
            {
                Username = "doesnotexist"
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "/api/User/delete")
            {
                Content = JsonContent.Create(request)
            };

            var response = await _client.SendAsync(httpRequest);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Delete_User_Fail_Admin_Deletes_Self()
        {
            // The token is generated with username "selfadmin"
            var token = _factory.GenerateTestToken(UserProfile.Administrator, "selfadmin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.BadRequest;

            var request = new UserDeletionRequest
            {
                Username = "selfadmin"
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "/api/User/delete")
            {
                Content = JsonContent.Create(request)
            };

            var response = await _client.SendAsync(httpRequest);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Get_Users_Success_Admin_Role()
        {
            var token = _factory.GenerateTestToken(UserProfile.Administrator);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.OK;

            var response = await _client.GetAsync("/api/User/get/users");

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Get_Users_Fail_Manager_Role()
        {
            var token = _factory.GenerateTestToken(UserProfile.Manager);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.Forbidden;

            var response = await _client.GetAsync("/api/User/get/users");

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Get_Users_Fail_Stocktaker_Role()
        {
            var token = _factory.GenerateTestToken(UserProfile.Stocktaker);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.Forbidden;

            var response = await _client.GetAsync("/api/User/get/users");

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Get_Users_Fail_No_Token()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var expected = HttpStatusCode.Unauthorized;

            var response = await _client.GetAsync("/api/User/get/users");

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Change_Own_Password_Manager_Role_Success()
        {
            _factory.ResetDatabase();

            var adminToken = _factory.GenerateTestToken(UserProfile.Administrator);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            await _client.PostAsJsonAsync("/api/User/create", new UserDto
            {
                Username = "manageruser",
                Password = "OldPassword123!",
                Profile = UserProfile.Manager
            });

            var token = _factory.GenerateTestToken(UserProfile.Manager, "manageruser");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.OK;

            var request = new ChangePasswordRequest
            {
                Username = "manageruser",
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            var response = await _client.PutAsJsonAsync("/api/User/change-password", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Change_Own_Password_Admin_Role_Success()
        {
            _factory.ResetDatabase();

            var adminToken = _factory.GenerateTestToken(UserProfile.Administrator);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            await _client.PostAsJsonAsync("/api/User/create", new UserDto
            {
                Username = "otheradmin",
                Password = "OldPassword123!",
                Profile = UserProfile.Administrator
            });

            var token = _factory.GenerateTestToken(UserProfile.Administrator, "otheradmin");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.OK;

            var request = new ChangePasswordRequest
            {
                Username = "otheradmin",
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            var response = await _client.PutAsJsonAsync("/api/User/change-password", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Change_Own_Password_Stocktaker_Role_Success()
        {
            _factory.ResetDatabase();

            var adminToken = _factory.GenerateTestToken(UserProfile.Administrator);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            await _client.PostAsJsonAsync("/api/User/create", new UserDto
            {
                Username = "stocktakeruser",
                Password = "OldPassword123!",
                Profile = UserProfile.Stocktaker
            });

            var token = _factory.GenerateTestToken(UserProfile.Stocktaker, "stocktakeruser");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.OK;

            var request = new ChangePasswordRequest
            {
                Username = "stocktakeruser",
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            var response = await _client.PutAsJsonAsync("/api/User/change-password", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Admin_Changes_Other_User_Password_Success()
        {
            _factory.ResetDatabase();

            var adminToken = _factory.GenerateTestToken(UserProfile.Administrator);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            await _client.PostAsJsonAsync("/api/User/create", new UserDto
            {
                Username = "targetuser",
                Password = "OldPassword123!",
                Profile = UserProfile.Manager
            });

            var expected = HttpStatusCode.OK;

            var request = new ChangePasswordRequest
            {
                Username = "targetuser",
                CurrentPassword = "TheCurrentPassword", // not checked when changing someone else's password
                NewPassword = "NewPassword123!"
            };

            var response = await _client.PutAsJsonAsync("/api/User/change-password", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task Manager_Change_Other_User_Password_Fail()
        {
            _factory.ResetDatabase();

            var adminToken = _factory.GenerateTestToken(UserProfile.Administrator);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            await _client.PostAsJsonAsync("/api/User/create", new UserDto
            {
                Username = "manageruser2",
                Password = "OldPassword123!",
                Profile = UserProfile.Manager
            });

            await _client.PostAsJsonAsync("/api/User/create", new UserDto
            {
                Username = "victimuser",
                Password = "OldPassword123!",
                Profile = UserProfile.Stocktaker
            });

            var token = _factory.GenerateTestToken(UserProfile.Manager, "manageruser2");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.Forbidden;

            var request = new ChangePasswordRequest
            {
                Username = "victimuser",
                CurrentPassword = "TheCurrentPassword",
                NewPassword = "NewPassword123!"
            };

            var response = await _client.PutAsJsonAsync("/api/User/change-password", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_Fail_No_Token()
        {
            _client.DefaultRequestHeaders.Authorization = null;

            var expected = HttpStatusCode.Unauthorized;

            var request = new ChangePasswordRequest
            {
                Username = "someuser",
                CurrentPassword = "OldPassword123!",
                NewPassword = "NewPassword123!"
            };

            var response = await _client.PutAsJsonAsync("/api/User/change-password", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_Fail_Incorrect_Current_Password()
        {
            _factory.ResetDatabase();

            var adminToken = _factory.GenerateTestToken(UserProfile.Administrator);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            await _client.PostAsJsonAsync("/api/User/create", new UserDto
            {
                Username = "manageruser3",
                Password = "OldPassword123!",
                Profile = UserProfile.Manager
            });

            var token = _factory.GenerateTestToken(UserProfile.Manager, "manageruser3");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var expected = HttpStatusCode.BadRequest;

            var request = new ChangePasswordRequest
            {
                Username = "manageruser3",
                CurrentPassword = "WrongPassword!",
                NewPassword = "NewPassword123!"
            };

            var response = await _client.PutAsJsonAsync("/api/User/change-password", request);

            Assert.Equal(expected, response.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_Fail_User_Not_Found()
        {
            _factory.ResetDatabase();

            var adminToken = _factory.GenerateTestToken(UserProfile.Administrator);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            var expected = HttpStatusCode.NotFound;

            var request = new ChangePasswordRequest
            {
                Username = "doesnotexist",
                CurrentPassword = "TheCurrentPassword",
                NewPassword = "NewPassword123!"
            };

            var response = await _client.PutAsJsonAsync("/api/User/change-password", request);

            Assert.Equal(expected, response.StatusCode);
        }
    }
}