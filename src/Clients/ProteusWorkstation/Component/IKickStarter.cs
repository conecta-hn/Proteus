/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Pages.Base;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// definir la configuración de arranque de Proteus Workstation.
    /// </summary>
    public interface IKickStarter
    {
        /// <summary>
        /// Obtiene la página de inicio de sesión para este
        /// <see cref="IKickStarter"/>.
        /// </summary>
        /// <returns>
        /// Una página de inicio de sesión válida para la aplicación.
        /// </returns>
        ILoginPage GetLoginPage();

        /// <summary>
        /// Obtiene la página prinicpal de la aplicaicón.
        /// </summary>
        /// <returns>
        /// Una pagina a utilizar como la página principal de la 
        /// aplicación.
        /// </returns>
        IPage GetMainPage();

        /// <summary>
        /// Obtiene la página de configuración de la aplicación.
        /// </summary>
        /// <returns>
        /// Una página a utilizar como la página de configuración de la
        /// aplicación.
        /// </returns>
        IPage GetSettingsPage();

        /// <summary>
        /// Indica si este <see cref="IKickStarter"/> habilita o
        /// deshabilita el subsistema de usuarios de Proteus.
        /// </summary>
        /// <value>
        /// <see langword="true"/> activa el sistem de usuarios de Proteus.
        /// <see langword="false"/> lo deshabilita y permite accesar a toda
        /// la funcionalidad sin requerir el inicio de sesión.
        /// </value>
        bool RequiresInteractiveLogin { get; }

        /// <summary>
        /// Indica si es posible usar o no este <see cref="IKickStarter"/>.
        /// </summary>
        bool Usable { get; }
    }
}