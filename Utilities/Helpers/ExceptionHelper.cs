using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Utilities.Exceptions;

namespace Utilities.Helpers
{
    public static class ExceptionHelper
    {
        public static Exception HandleDbException(Exception ex)
        {
            if (ex is DbUpdateConcurrencyException)
                return new DataException("Error de concurrencia en la base de datos.", ex);

            if (ex is DbUpdateException)
                return new DataException("Error al actualizar la base de datos.", ex);

            if (ex.InnerException != null)
            {
                if (ex.InnerException.Message.Contains("could not connect") ||
                    ex.InnerException.Message.Contains("unable to connect") ||
                    ex.InnerException.Message.Contains("connection refused"))
                {
                    return new DatabaseConnectionException("No se pudo conectar a la base de datos.", ex);
                }
            }

            return new DataException("Ha ocurrido un error en la capa de datos.", ex);
        }

    }
}
