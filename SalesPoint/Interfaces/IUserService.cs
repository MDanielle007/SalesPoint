using SalesPoint.DTO;

namespace SalesPoint.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> RegisterUserAsync(UserCreateDTO userDTO);
        Task<AuthResponseDTO> LoginUserAsync(LoginDTO loginDTO);
        Task<UserDTO> GetUserByIdAsync(int id);
        Task<IEnumerable<UserDTO>> GetUsersAsync();
        Task<UserDTO> UpdateUserAsync(UserUpdateDTO userDTO);
        Task DeleteUserAsync(int id);
        Task ChangePassword(int userId, ChangePasswordDTO changePasswordDTO);
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
    }
}
