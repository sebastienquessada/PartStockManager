using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartStockManager.API.DTOs;
using PartStockManager.CoreLogic.Models;
using PartStockManager.CoreLogic.Repositories;

namespace PartStockManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _logger = logger;
            _configuration = configuration;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [Route("create")]
        public IActionResult CreateUser([FromBody] UserDto request)
        {
            _logger.LogInformation("API Request: CreateUser for username {Username}", request.Username);

            if (string.IsNullOrEmpty(request.Username))
            {
                _logger.LogWarning("CreateUser failed: the username is missing!");
                return BadRequest("Username is required.");
            }

            try
            {
                var newUser = new User(
                    request.Username,
                    BCrypt.Net.BCrypt.HashPassword(request.Password),
                    request.Profile
                );

                var result = _userRepository.CreateUser(newUser);

                return result ? Ok($"User successfully created (Username: '{newUser.Username}')") : Conflict($"A user with the username '{request.Username}' already exists!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API Request - CreateUser: An error occurred during CreateUser for {Username}", request.Username);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPut]
        [Route("modify")]
        public IActionResult ModifyUser([FromBody] UserModificationRequest request)
        {
            _logger.LogInformation("API Request: ModifyUser for username {Username}", request.CurrentUsername);

            try
            {
                var currentUser = _userRepository.GetByUsername(request.CurrentUsername);

                if (currentUser == null)
                    return NotFound($"No user with the username '{request.CurrentUsername}'");

                var defaultAdminUsername = _configuration["Seed:AdminUsername"];

                if (request.CurrentUsername == defaultAdminUsername)
                {
                    _logger.LogWarning("ModifyUser failed: attempt to modify the default administrator account {Username}", request.CurrentUsername);
                    return BadRequest("The default administrator account cannot be modified.");
                }

                // Prevent an administrator from modifying their own rights
                var currentUsername = User.Identity?.Name;

                if (currentUsername == request.CurrentUsername && request.UpdatedUser.Profile != currentUser.Profile)
                {
                    _logger.LogWarning("ModifyUser failed: administrator {Username} tried to modify their own rights", currentUsername);
                    return BadRequest("You cannot modify your own rights.");
                }

                var updatedUser = new User(
                    request.UpdatedUser.Username,
                    BCrypt.Net.BCrypt.HashPassword(request.UpdatedUser.Password),
                    request.UpdatedUser.Profile
                );

                var result = _userRepository.ModifyUser(request.CurrentUsername, updatedUser);

                return result ? Ok($"User successfully modified (Old Username: '{request.CurrentUsername}', Current Username: '{updatedUser.Username}')") : NotFound($"Error during user update (Username: '{request.CurrentUsername}')");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API Request - ModifyUser: An error occurred during ModifyUser for {Username}", request.CurrentUsername);

                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete]
        [Route("delete")]
        public IActionResult DeleteUser([FromBody] UserDeletionRequest request)
        {
            _logger.LogInformation("API Request: DeleteUser for username {Username}", request.Username);

            // Prevent an administrator from deleting themselves
            var currentUsername = User.Identity?.Name;

            if (currentUsername == request.Username)
            {
                _logger.LogWarning("DeleteUser failed: administrator {Username} tried to delete themselves", currentUsername);

                return BadRequest("You cannot delete your own account.");
            }

            var defaultAdminUsername = _configuration["Seed:AdminUsername"];

            if (request.Username == defaultAdminUsername)
            {
                _logger.LogWarning("DeleteUser failed: attempt to delete the default administrator account {Username}", request.Username);

                return BadRequest("The default administrator account cannot be deleted.");
            }

            try
            {
                var result = _userRepository.DeleteUser(request.Username);

                return result ? Ok($"User successfully deleted (Username: '{request.Username}')") : NotFound($"No user with the username '{request.Username}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API Request - DeleteUser: An error occurred during DeleteUser for {Username}", request.Username);

                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("get/users")]
        public IActionResult GetUsers()
        {
            _logger.LogInformation("API Request: GetUsers");

            try
            {
                var result = _userRepository.GetAll();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API Request - GetUsers: An error occurred when retrieving users");
                return StatusCode(500, "Internal server error while retrieving users.");
            }
        }

        [Authorize]
        [HttpPut]
        [Route("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var currentUsername = User.Identity?.Name;
            var isAdministrator = User.IsInRole("Administrator");

            // Determine which account is being targeted
            var targetUsername = string.IsNullOrEmpty(request.Username)
                ? currentUsername
                : request.Username;

            _logger.LogInformation("API Request: ChangePassword for {TargetUsername} by {CurrentUsername}", targetUsername, currentUsername);

            // A non-administrator can only change their own password
            if (!isAdministrator && targetUsername != currentUsername)
            {
                _logger.LogWarning("ChangePassword failed: {CurrentUsername} attempted to change another user's password", currentUsername);
                return Forbid();
            }

            try
            {
                var targetUser = _userRepository.GetByUsername(targetUsername);

                if (targetUser == null)
                    return NotFound("User not found.");

                // The current password is required only when changing one's own password
                if (targetUsername == currentUsername)
                {
                    if (string.IsNullOrEmpty(request.CurrentPassword) || !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, targetUser.PasswordHash))
                    {
                        _logger.LogWarning("ChangePassword failed: incorrect current password for {Username}", currentUsername);

                        return BadRequest("Current password is incorrect.");
                    }
                }

                var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                var result = _userRepository.UpdatePassword(targetUsername!, newHash);

                return result ? Ok("Password successfully changed.") : StatusCode(500, "Internal Server Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "API Request - ChangePassword: An error occurred for {TargetUsername}", targetUsername);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}