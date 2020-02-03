/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Config
{
    /// <summary>
    /// Describe una serie de propiedades utilizadas para configurar
    /// Proteus.
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// Obtiene el nombre de la empresa registrada a utilizar el sistema.
        /// </summary>
        string BusinessName { get; }

        /// <summary>
        /// Indica si se debe utilizar un servidor de datos de dominio.
        /// </summary>
        bool UseDomainProvider { get; }

        /// <summary>
        /// Nombre/dirección IP del servidor de datos del dominio.
        /// </summary>
        string DomainProvider { get; }

        /// <summary>
        /// Indica si se debe utilizar una instancia de SQL Server LocalDB
        /// como base de datos.
        /// </summary>
        bool UseLocalDbProvider { get; }

        /// <summary>
        /// Indica si se debe utilizar una cadena de conexión personalizada
        /// para conectarse a un servidor de base de datos.
        /// </summary>
        bool UseCustomProvider { get; }

        /// <summary>
        /// Cadena de conexión personalizada para conectarse a un servidor
        /// de base de datos.
        /// </summary>
        string CustomProvider { get; }

        /// <summary>
        /// indica si el servicio de red de Proteus está habilitado.
        /// </summary>
        bool UseNetworkServer { get; }

        /// <summary>
        /// Nombre/dirección IP del equipo que provee el servicio de red de
        /// Proteus.
        /// </summary>
        string NetworkServerAddress { get; }

        /// <summary>
        /// Puerto del servicio de red de Proteus.
        /// </summary>
        int NetworkServerPort { get; }

        /// <summary>
        /// Directorio genérico de Plugins.
        /// </summary>
        string PluginsDir { get; }

        /// <summary>
        /// Indica el modo de inicialización de Proteus.
        /// </summary>
        Proteus.InitMode InitMode { get; }

        /// <summary>
        /// Indica el tiempo en milisegundos de espera al intentar
        /// comunicarse con un servidor de base de datos.
        /// </summary>
        int ServerTimeout { get; }

        /// <summary>
        /// Indica si es necesaria una conexión exitosa con el servidor de
        /// red para completar el inicio de sesión en Proteus.
        /// </summary>
        bool RequireNetworkServerSuccess { get; }

        /// <summary>
        /// Configura Proteus para comprobar la existencia de Entidades con
        /// el mismo Id a nivel de la aplicación en lugar de delegarle la
        /// operación al servidor de datos.
        /// </summary>
        bool CheckExists { get; }

        /// <summary>
        /// Habilita la función de escucha de anuncio del servidor de red
        /// para recuperar la conectividad cuando esta se pierda.
        /// </summary>
        bool EnableAnnounce { get; }

        /// <summary>
        /// Obtiene un límite de selección de columnas para las funciones que
        /// lo soporten.
        /// </summary>
        int RowLimit { get; }
    }
}