using SalesPoint.DTO;

namespace SalesPoint.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> RegisterUserAsync(UserCreateDTO userDTO);
        Task<AuthResponseDTO> LoginUserAsync(LoginDTO loginDTO);
        Task<UserDTO> GetUserByIdAsync(string id);
        Task<IEnumerable<UserDTO>> GetUsersAsync();
        Task<UserDTO> UpdateUserAsync(UserUpdateDTO userDTO);
        Task DeleteUserAsync(string id);
        Task ChangePassword(string userId, ChangePasswordDTO changePasswordDTO);
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
    }
}
