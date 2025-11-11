using Entity.Dtos;
using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IUserBusiness : IRepositoryBusiness<User, UserDto>
    {
        Task<UserDto> LoginUser(LoginRequestDto loginDto);
        Task<UserDto> RegisterAsync(RegisterRequestDto dto);
        Task<IEnumerable<UserDto>> GetAllJoinAsync();
    }
}
