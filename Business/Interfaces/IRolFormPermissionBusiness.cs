using Entity.DTOs;
using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IRolFormPermissionBusiness : IRepositoryBusiness<RolFormPermission, RolFormPermissionDto>
    {

        Task<IEnumerable<RolFormPermissionDto>> GetAllJoinAsync();

    }

}
