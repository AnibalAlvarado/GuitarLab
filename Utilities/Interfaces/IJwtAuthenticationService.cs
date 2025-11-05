using Entity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Interfaces
{
    public interface IJwtAuthenticationService
    {
        string GenerarToken(Guitarist usuario, List<string> roles);
    }
}
