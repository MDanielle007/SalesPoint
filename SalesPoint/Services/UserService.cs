using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SalesPoint.DTO;
using SalesPoint.Exceptions;
using SalesPoint.Interfaces;
using SalesPoint.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SalesPoint.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ILogger<UserService> _logger;
        private readonly UserManager<User> _userManager;


        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IConfiguration config,
            ILogger<UserService> logger,
            UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _config = config;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<UserDTO> RegisterUserAsync(UserCreateDTO userDTO)
        {
            try
            {
                var existingByUsername = await _userManager.FindByNameAsync(userDTO.Username);
                if (existingByUsername != null)
                    throw new BadRequestException("Username already exists");

                var existingByEmail = await _userManager.FindByEmailAsync(userDTO.Email);
                if (existingByEmail != null)
                    throw new BadRequestException("Email already exists");

                if (await _userRepository.EmployeeIdExistsAsync(userDTO.EmployeeId))
                    throw new BadRequestException("Employee ID already exists");

                var user = new User
                {
                    UserName = userDTO.Username,
                    Email = userDTO.Email,
                    Role = userDTO.Role,
                    EmployeeId = userDTO.EmployeeId,
                    FirstName = userDTO.FirstName,
                    MiddleName = userDTO.MiddleName,
                    LastName = userDTO.LastName,
                    PhoneNumber = userDTO.Phone,
                };

                var result = await _userManager.CreateAsync(user, userDTO.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception("User creation failed: " + errors);
                }

                await _userManager.AddToRoleAsync(user, userDTO.Role.ToString());

                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                throw;
            }
        }

        public async Task<AuthResponseDTO> LoginUserAsync(LoginDTO loginDTO)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginDTO.Username);
                if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
                    throw new UnauthorizedException("Invalid username or password");

                var token = await GenerateJwtToken(user);

                return new AuthResponseDTO
                {
                    Token = token,
                    Expiration = DateTime.Now.AddHours(Convert.ToDouble(_config["Jwt:ExpireHours"])),
                    User = _mapper.Map<UserDTO>(user),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                throw;
            }
        }

        public async Task<UserDTO> GetUserByIdAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if(user == null) throw new NotFoundException("User not found");
                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user with id: {id}");
                throw;
            }
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync()
        {
            try
            {
                var users = await _userRepository.GetUsersAsync();
                return _mapper.Map<IEnumerable<UserDTO>>(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                throw;
            }
        }

        public async Task<UserDTO> UpdateUserAsync(UserUpdateDTO userDTO)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userDTO.Id.ToString());
                if (user == null) throw new NotFoundException("User not found");

                if (!string.IsNullOrEmpty(userDTO.Email) && user.Email != userDTO.Email)
                {
                    var emailExists = await _userManager.FindByEmailAsync(userDTO.Email);
                    if (emailExists != null && emailExists.Id != user.Id)
                        throw new BadRequestException("Email already exists");

                    user.Email = userDTO.Email;
                }

                if (!string.IsNullOrEmpty(userDTO.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetResult = await _userManager.ResetPasswordAsync(user, token, userDTO.Password);

                    if (!resetResult.Succeeded)
                        throw new BadRequestException("Password update failed: " + string.Join(", ", resetResult.Errors.Select(e => e.Description)));
                }

                if (userDTO.Role.HasValue && !await _userManager.IsInRoleAsync(user, userDTO.Role.Value.ToString()))
                {
                    // Remove current roles
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                    // Assign new role
                    await _userManager.AddToRoleAsync(user, userDTO.Role.Value.ToString());
                }

                user.EmployeeId = userDTO.EmployeeId ?? user.EmployeeId;
                user.FirstName = userDTO.FirstName ?? user.FirstName;
                user.MiddleName = userDTO.MiddleName ?? user.MiddleName;
                user.LastName = userDTO.LastName ?? user.LastName;
                user.PhoneNumber = userDTO.Phone ?? user.PhoneNumber;

                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                    throw new Exception("User update failed: " + string.Join(", ", updateResult.Errors.Select(e => e.Description)));

                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user with id: {userDTO.Id}");
                throw;
            }
        }

        public async Task DeleteUserAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                    throw new NotFoundException("User not found");

                user.DeletedAt = DateTime.Now;
                await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user with id: {id}");
                throw;
            }
        }

        public async Task ChangePassword(string userId, ChangePasswordDTO dto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    throw new NotFoundException("User not found");

                var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
                if (!result.Succeeded)
                    throw new BadRequestException("Failed to change password: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing password of user {userId}");
                throw;
            }
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                return user != null && await _userManager.CheckPasswordAsync(user, password);
            }
             catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating user with username: {username}");
                throw;
            }
        }

        #region Private Methods

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            var hashOfInput = HashPassword(inputPassword);
            return hashOfInput == storedHash;   
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "Unknown")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(Convert.ToDouble(_config["Jwt:ExpireHours"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }
}
