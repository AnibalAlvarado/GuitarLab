using Entity.DTOs;
using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IRolUserBusiness : IRepositoryBusiness<RolUser, RolUserDto>
    {
        public Task<IEnumerable<RolUserDto>> GetAllJoinAsync();
        Task<bool> ExistsAsync(int userId, int roleId);
    }
}
