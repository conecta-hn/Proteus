/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using TheXDS.Proteus.Pages;
using TheXDS.Proteus.Pages.Base;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.Config
{
    /// <summary>
    ///     Contiene la configuración predeterminada de Proteus.
    /// </summary>
    public class LaunchConfig : IKickStarter
    {
        /// <summary>
        ///     Cambia el comportamiento de inicio de sesión de Proteus.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> activa el sistem de usuarios de Proteus.
        ///     <see langword="false"/> lo deshabilita y permite accesar a toda
        ///     la funcionalidad sin requerir el inicio de sesión.
        /// </value>
        public bool RequiresInteractiveLogin => false;

        /// <summary>
        ///     Indica si es posible usar o no este <see cref="IKickStarter"/>.
        /// </summary>
        public bool Usable => true;

        /// <summary>
        ///     Obtiene la página de inicio de sesión para este
        ///     <see cref="IKickStarter"/>.
        /// </summary>
        /// <returns>
        ///     Una página de inicio de sesión válida para la aplicación.
        /// </returns>
        public ILoginPage GetLoginPage()
        {
            return new SimpleLoginPage();
        }

        /// <summary>
        ///     Obtiene la página prinicpal de la aplicaicón.
        /// </summary>
        /// <returns>
        ///     Una pagina a utilizar como la página principal de la 
        ///     aplicación.
        /// </returns>
        public IPage GetMainPage()
        {
            return new DashboardPage();
        }

        /// <summary>
        ///     Obtiene la página de configuración de la aplicación.
        /// </summary>
        /// <returns>
        ///     Una página a utilizar como la página de configuración de la
        ///     aplicación.
        /// </returns>
        public IPage GetSettingsPage()
        {
            return new SettingsPage();
        }
    }
}