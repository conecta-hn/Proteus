/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Models.Base
{
    /// <summary>
    /// Describe una serie de miembros a implementar por un tipo que exponga
    /// información de dirección física.
    /// </summary>
    public interface IAddressable : IAddressArea
    {
        /// <summary>
        /// Obtiene o establece la dirección de la entidad.
        /// </summary>
        string Address { get; set; }
    }
}