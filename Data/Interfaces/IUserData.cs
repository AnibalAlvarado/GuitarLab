using Entity.Dtos;
using Entity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IUserData : IRepositoryData<User>
    {
        Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername);
    }
}
