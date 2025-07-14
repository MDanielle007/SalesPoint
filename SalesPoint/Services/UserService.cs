using AutoMapper;
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
        private readonly ILogger _logger;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            IConfiguration config,
            ILogger logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _config = config;
            _logger = logger;
        }

        public async Task<UserDTO> RegisterUserAsync(UserCreateDTO userDTO)
        {
            try
            {
                if(await _userRepository.UsernameExistsAsync(userDTO.Username)){
                    throw new BadRequestException("Username already exists");
                }
                if (await _userRepository.EmailExistsAsync(userDTO.Email)){
                    throw new BadRequestException("Email already exists");
                }
                if (await _userRepository.EmployeeIdExistsAsync(userDTO.EmployeeId)){
                    throw new BadRequestException("Employee ID already exists");
                }

                var user = _mapper.Map<User>(userDTO);
                user.Password = HashPassword(userDTO.Password);

                var createdUser = await _userRepository.AddUserAsync(user);
                return _mapper.Map<UserDTO>(createdUser);
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
                var user = await _userRepository.GetUserByUsernameAsync(loginDTO.Username);
                if( user == null || !VerifyPassword(loginDTO.Password, user.Password))
                {
                    throw new UnauthorizedException("Invalid username or password");
                }

                var token = GenerateJwtToken(user);

                return new AuthResponseDTO
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddHours(Convert.ToDouble(_config["Jwt:ExpireHours"])),
                    User = _mapper.Map<UserDTO>(user),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                throw;
            }
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);
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
                var user = await _userRepository.GetUserByIdAsync(userDTO.Id);
                if (user == null) throw new NotFoundException("User not found");

                if(!string.IsNullOrEmpty(userDTO.Email) && user.Email != userDTO.Email)
                {
                    if(await _userRepository.EmailExistsAsync(userDTO.Email))
                    {
                        throw new BadRequestException("Email already exist");
                    }
                    user.Email = userDTO.Email;
                }

                if (!string.IsNullOrEmpty(userDTO.Password))
                {
                    user.Password = HashPassword(userDTO.Password);
                }

                if (userDTO.Role.HasValue)
                {
                    user.Role = userDTO.Role.Value;
                }

                if (!string.IsNullOrEmpty(userDTO.EmployeeId))
                {
                    user.EmployeeId = userDTO.EmployeeId;
                }

                if (!string.IsNullOrEmpty(userDTO.FirstName))
                {
                    user.FirstName = userDTO.FirstName;
                }

                if (!string.IsNullOrEmpty(userDTO.MiddleName))
                {
                    user.MiddleName = userDTO.MiddleName;
                }

                if (!string.IsNullOrEmpty(userDTO.LastName))
                {
                    user.LastName = userDTO.LastName;
                }

                if (!string.IsNullOrEmpty(userDTO.Phone))
                {
                    user.Phone = userDTO.Phone;
                }

                await _userRepository.UpdateUserAsync(user);
                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user with id: {userDTO.Id}");
                throw;
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                if (user != null) throw new NotFoundException("User not found");
                await _userRepository.DeleteUserAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user with id: {id}");
                throw;
            }
        }

        public async Task ChangePassword(int userId, ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user != null) throw new NotFoundException("User not found");
                if (!VerifyPassword(changePasswordDTO.CurrentPassword, user.Password)) throw new BadRequestException("Current password is incorrect");
                
                user.Password = changePasswordDTO.CurrentPassword;
                await _userRepository.UpdateUserAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing password of user with id: {userId}");
                throw;
            }
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(username);
                return user != null && VerifyPassword(password, user.Password);
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

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
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
