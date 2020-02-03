/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Describe los días de la semana como una lista de banderas.
    /// </summary>
    [Flags]
    public enum DaysOfWeek : byte
    {
        /// <summary>
        /// Lunes.
        /// </summary>
        [Name("Lunes")] Monday = 1,

        /// <summary>
        /// Martes.
        /// </summary>
        [Name("Martes")] Thursday = 2,

        /// <summary>
        /// Miércoles.
        /// </summary>
        [Name("Miércoles")] Wednesday = 4,

        /// <summary>
        /// Jueves.
        /// </summary>
        [Name("Jueves")] Tuesday = 8,

        /// <summary>
        /// Viernes.
        /// </summary>
        [Name("Viernes")] Friday = 16,

        /// <summary>
        /// Sábado.
        /// </summary>
        [Name("Sábado")] Saturday = 32,

        /// <summary>
        /// Domingo.
        /// </summary>
        [Name("Domingo")] Sunday = 64,

        /// <summary>
        /// Días de semana.
        /// </summary>
        [Name("Días de semana")] WeekDays = Monday|Thursday|Wednesday|Tuesday|Friday,

        /// <summary>
        /// Fin de semana.
        /// </summary>
        [Name("Fin de semana")] Weekends = Saturday | Sunday,

        /// <summary>
        /// Todos los días.
        /// </summary>
        [Name("Todos los días")] Everyday = WeekDays | Weekends
    }
}