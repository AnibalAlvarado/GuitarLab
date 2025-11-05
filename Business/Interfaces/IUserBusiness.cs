using Entity.Dtos;
using Entity.DTOs;
using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IUserBusiness : IRepositoryBusiness<User, UserDto>
    {
        Task<UserResponseDto?> ValidateUserAsync(string username, string password);
        Task AssignDefaultRoleAsync(int userId);
        Task SendWelcomeEmailAsync(string to);
    }
}
