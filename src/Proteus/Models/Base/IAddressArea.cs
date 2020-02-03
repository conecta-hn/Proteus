/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Models.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que exponga 
    /// información de dirección física detallada.
    /// </summary>
    public interface IAddressArea
    {
        /// <summary>
        /// Obtiene o establece la cuidad de ubicación.
        /// </summary>
        string City { get; set; }

        /// <summary>
        /// Obtiene o establece el país de ubicación.
        /// </summary>
        string Country { get; set; }

        /// <summary>
        /// Obtiene o establece la provincia/departamento/estado o equivalente
        /// de ubicación.
        /// </summary>
        string Province { get; set; }

        /// <summary>
        /// Obtiene o establece el código Zip opcional de la ubicación.
        /// </summary>
        string Zip { get; set; }
    }
}