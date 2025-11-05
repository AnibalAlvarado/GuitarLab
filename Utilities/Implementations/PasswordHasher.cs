using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Interfaces;
using Microsoft.AspNetCore.Identity;
using Entity.Models;

namespace Utilities.Implementations
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<Guitarist> _hasher = new();

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var result = _hasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
