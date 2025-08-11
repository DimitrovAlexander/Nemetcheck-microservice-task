using UserMicroservice.Data.DTOs;

namespace UserMicroservice.Services.UserService
{
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(RegisterDTO dto);
        Task<string> LoginAsync(LoginDto dto);
    }
}
