/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    /// Enumeración que describe los permisos que pueden ser requeridos por un módulo, u otorgados a un usuario.
    /// </summary>
    [Flags]
    public enum SecurityFlags
    {
        /// <summary>
        /// Deniega todos los permisos actuales y futuros. Además, representa el permiso predeterminado nulo.
        /// </summary>
        None,

        /// <summary>
        /// Requiere/otorga el permiso para ver un elemento en la UI.
        /// </summary>
        View,

        /// <summary>
        /// Requiere/otorga el permiso para listar información.
        /// </summary>
        Open,

        /// <summary>
        /// Requiere/otorga el permiso para realizar búsquedas de información.
        /// </summary>
        Search = 4,

        /// <summary>
        /// Requiere/otorga los permisos estándar de lectura.
        /// </summary>
        Read = View | Open | Search,

        /// <summary>
        /// Requiere/otorga el permiso para crear nueva información.
        /// </summary>
        New,

        /// <summary>
        /// Requiere/otorga el permiso para editar información.
        /// </summary>
        Edit = 16,

        /// <summary>
        /// Requiere/otorga el permiso para borrar información.
        /// </summary>
        /// <remarks>
        /// Contrario a purgar, borrar únicamente marca entidades como
        /// borradas, por lo que es posible restaurarlas cambiando su
        /// estado. 
        /// </remarks>
        Delete = 32,

        /// <summary>
        /// Requiere/otorga los permisos estándar de escritura.
        /// </summary>
        Write = New | Edit | Delete,

        /// <summary>
        /// Requiere/otorga los permisos estándar de lectura y escritura.
        /// </summary>
        ReadWrite = Read | Write,

        /// <summary>
        /// Requiere/otorga el permiso para purgar información.
        /// </summary>
        /// <remarks>
        /// Contrario a borrar, purgar elimina permanentemente la
        /// información de la base de datos, por lo que no es posible
        /// recuperarla. 
        /// </remarks>
        Purge = 64,

        /// <summary>
        /// Requiere/otorga permisos administrativos estándar.
        /// </summary>
        Admin = 128,

        /// <summary>
        /// Requiere/otorga todos los permisos administrativos.
        /// </summary>
        FullAdmin = ReadWrite | Purge | Admin,

        /// <summary>
        /// Requiere/otorga el permiso de elevación de permisos.
        /// </summary>
        Elevate = int.MinValue,

        /// <summary>
        /// Requiere/otorga todos los permisos actuales y futuros. (aplicar a superusuarios)
        /// </summary>
        Root = -1
    }
}
