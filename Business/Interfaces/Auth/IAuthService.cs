using Entity.Dtos;
namespace Business.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<UserDto> RegisterAsync(RegisterRequestDto dto);
    }
}
