using Utilities.Interfaces;

namespace Utilities.Implementations
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int DefaultWorkFactor = 12;

        public string HashPassword(string password)
        {
            // Genera el hash con el work factor definido
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: DefaultWorkFactor);
        }

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            // Verifica si la contraseña coincide con el hash
            return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
        }
    }
}
