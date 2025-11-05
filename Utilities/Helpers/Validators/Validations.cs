using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Helpers.Validators
{
    public class Validations
    {
        public Task<bool> SizeNumValidation(string num, int size)
        {
            if (num.Length > size)
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }

        public Task<bool> DocumentValidation(string documento)
        {
            if (documento.Length < 8 || documento.Length > 10)
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }

        public void ValidateDto<T>(T dto, params string[] includedProperties)
        {
            if (dto == null)
                throw new ArgumentException($"{typeof(T).Name} cannot be null.");

            foreach (var prop in typeof(T).GetProperties())
            {
                // Si la propiedad NO está en la lista de las que se quieren validar, se salta
                if (includedProperties.Length > 0 && !includedProperties.Contains(prop.Name))
                    continue;

                var value = prop.GetValue(dto);

                if (value == null)
                    throw new ArgumentException($"The field {prop.Name} cannot be null.");

                if (prop.PropertyType == typeof(string) && string.IsNullOrWhiteSpace((string)value))
                    throw new ArgumentException($"The field {prop.Name} cannot be empty.");

                if (prop.PropertyType == typeof(int) && (int)value <= 0)
                    throw new ArgumentException($"The field {prop.Name} must be greater than 0.");

                if (prop.PropertyType == typeof(long) && (long)value <= 0)
                    throw new ArgumentException($"The field {prop.Name} must be greater than 0.");

                if (prop.PropertyType == typeof(string) && prop.Name.ToLower().Contains("hora"))
                {
                    if (!TimeSpan.TryParseExact((string)value, @"hh\:mm\:ss", null, out TimeSpan timeSpanValue))
                    {
                        throw new ArgumentException($"The field {prop.Name} must have the format HH:mm:ss.");
                    }

                    // Validación del rango de la hora
                    if (timeSpanValue.Ticks <= 0 || timeSpanValue > TimeSpan.FromHours(24))
                    {
                        throw new ArgumentException($"The field {prop.Name} must be a valid time and cannot exceed 24 hours.");
                    }
                }

            }
        }

        public Task<bool> ValidDates(string fechaInicio, string fechaFin)
        {
            DateTime inicio, fin;

            if (DateTime.TryParseExact(fechaInicio, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out inicio) &&
                DateTime.TryParseExact(fechaFin, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fin))
            {
                return Task.FromResult(inicio < fin);
            }

            // Retorna false si las fechas no son válidas
            return Task.FromResult(false);
        }


        public Task<bool> IsValidDate(string fecha)
        {
            bool isValid = DateTime.TryParseExact(fecha, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
            return Task.FromResult(isValid);
        }

        public Task<bool> IsValidHour(string tick)
        {
            bool isValid = DateTime.TryParseExact(tick, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
            return Task.FromResult(isValid);
        }
    }
}
