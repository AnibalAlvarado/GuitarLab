using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ForeignIncludeAttribute : Attribute
    {
        /// <summary>
        /// Lista de rutas de propiedades que se desean incluir desde la propiedad de navegación.
        /// Ejemplo: "Nombre", "Direccion.Ciudad"
        /// </summary>
        public string[]? SelectPaths { get; }

        /// <summary>
        /// Constructor que acepta múltiples rutas opcionales.
        /// </summary>
        /// <param name="selectPaths">Una o más rutas anidadas a incluir desde la propiedad de navegación.</param>
        public ForeignIncludeAttribute(params string[] selectPaths)
        {   
            SelectPaths = selectPaths;
        }
    }
}
