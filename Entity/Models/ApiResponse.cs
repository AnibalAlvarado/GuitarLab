using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models
{
    ///<summary>
    /// Clase genérica para estandarizar las respuestas de la API
    /// </summary>
    /// <typeparam name="T">Tipo de datos que se incluirán en la respuesta</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Datos devueltos por la respuesta
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Indica si la operación fue exitosa
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje descriptivo sobre el resultado de la operación
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Información adicional o detalles de error (opcional)
        /// </summary>
        public object Details { get; set; }

        /// <summary>
        /// Constructor de la clase ApiResponse
        /// </summary>
        /// <param name="data">Datos a devolver</param>
        /// <param name="success">Indicador de éxito</param>
        /// <param name="message">Mensaje descriptivo</param>
        /// <param name="details">Detalles adicionales</param>
        public ApiResponse(T data, bool success, string message, object details)
        {
            Data = data;
            Success = success;
            Message = message;
            Details = details;
        }
    }
}
