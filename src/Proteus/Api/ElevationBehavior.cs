/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    /// Enumera los distintos modos de elevación soportados por un servicio
    /// de Proteus.
    /// </summary>
    public enum ElevationBehavior
    {
        /// <summary>
        /// Elevación única.
        /// </summary>
        [Name("Elevación única")] Once,
        /// <summary>
        /// Mantener elevación.
        /// </summary>
        [Name("Elevación persistente")] Keep,
        /// <summary>
        /// mantener elevación por un periodo de tiempo.
        /// </summary>
        [Name("Elevación con TTL")] Timeout
    }
}