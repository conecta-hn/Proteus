/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

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
        Once,
        /// <summary>
        /// Mantener elevación.
        /// </summary>
        Keep,
        /// <summary>
        /// mantener elevación por un periodo de tiempo.
        /// </summary>
        Timeout
    }
}