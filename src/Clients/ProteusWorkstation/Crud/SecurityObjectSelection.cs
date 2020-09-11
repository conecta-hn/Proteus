/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Reflection;
using TheXDS.MCART.Types.Base;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Crud
{
    /// <summary>
    /// Modelo temporal que permite seleccionar entidades asegurables para
    /// configurar descriptores de acceso.
    /// </summary>
    /// <typeparam name="T">
    /// Tipo de objeto de seguridad a seleccionar.
    /// </typeparam>
    public class SecurityObjectSelection<T> : ModelBase<int> where T : class
    {
        /// <summary>
        /// Obtiene una referencia al objeto de seguridad actualmente 
        /// seleccionado.
        /// </summary>
        public T SecurityObject { get; internal set; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="SecurityObjectSelection{T}"/>.
        /// </summary>
        /// <param name="securityObject">
        /// Objeto de seguridad a representar.
        /// </param>
        public SecurityObjectSelection(T securityObject)
        {
            SecurityObject = securityObject;
        }
        public SecurityObjectSelection()
        {
            SecurityObject = null!;
        }

        /// <summary>
        /// Convierte esta instancia a una cadena que representa al objeto de
        /// seguridad actualmente seleccionado.
        /// </summary>
        /// <returns>
        /// El nombre del objeto de seguridad seleccionado.
        /// </returns>
        public override string ToString()
        {
            return SecurityObject switch
            {
                INameable n => n.Name,
                IDescriptible d => d.Description,
                MethodInfo m => $"{m.Name} ({m.ReflectedType!.Name})",
                _ => SecurityObject?.ToString()
            } ?? "N/A";

        }
    }
}