/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Models.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que defina
    /// valores porcentuales y/o monetarios estáticos para cualquier propósito.
    /// </summary>
    public interface IValuable
    {
        /// <summary>
        /// Obtiene o establece el valor porcentual de esta instancia.
        /// </summary>
        float? PercentValue { get; set; }

        /// <summary>
        /// Obtiene o establece el valor estático de esta instancia.
        /// </summary>
        decimal? StaticValue { get; set; }
    }
}