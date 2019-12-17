/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Protocols
{
    /// <summary>
    /// Comandos de operaciones en el servidor.
    /// </summary>
    public enum Command : byte
    {
        /// <summary>
        /// Comando especial de latido de corazón.
        /// </summary>
        Heartbeat,

        /// <summary>
        /// Registra a un cliente.
        /// </summary>
        Register,

        /// <summary>
        ///     Finaliza la sesión del cliente.
        /// </summary>
        Unregister,

        /// <summary>
        ///     Verificar un recurso.
        /// </summary>
        Check,

        /// <summary>
        ///     Bloquear un recurso.
        /// </summary>
        Lock, 

        /// <summary>
        ///     Desbloquear un recurso en uso.
        /// </summary>
        Unlock,

        /// <summary>
        ///     Informar a los demás hosts conectados que uno de sus módulos
        ///     debe refrescarse.
        /// </summary>
        ServiceRefresh,

        /// <summary>
        ///     Informar a los demás hosts conectados que una de sus páginas
        ///     debe refrescarse.
        /// </summary>
        PageRefresh,

        /// <summary>
        ///     Informar a los demás hosts conectados que debe actualizarse la 
        ///     información de un ViewModel.
        /// </summary>
        ViewModelRefresh,

        /// <summary>
        ///     Solicita un listado de usuarios activos.
        /// </summary>
        QueryUsers,

        /// <summary>
        ///     Solicita un listado de sesiones activas del usuario.
        /// </summary>
        QuerySession,

        /// <summary>
        ///     Envía una alerta a un canal de destino.
        /// </summary>
        AlertTo,

        /// <summary>
        ///     Obliga a desconectar a un usuario.
        /// </summary>
        Purge,

        /// <summary>
        ///     Enumera individualmente todas las conexiones a este servidor.
        /// </summary>
        Enumerate,

        /// <summary>
        ///     Cierra forzosamente una conexión concreta.
        /// </summary>
        Close,

        /// <summary>
        ///     Efectúa una comprobación de latencia sobre TCP para un cliente.
        /// </summary>
        Probe,

        /// <summary>
        ///     Bloquea nuevos intentos de inicio de sesión.
        /// </summary>
        Deaf,

        /// <summary>
        ///     Desbloquea al servidor para volver a atender intentos de inicio
        ///     de sesión.
        /// </summary>
        Listen,

        /// <summary>
        ///     Inicia el proceso de detención del servidor de manera remota.
        /// </summary>
        Shutdown,

        /// <summary>
        ///     Envía un Broadcast UDP informando de la disponibilidad de este 
        ///     servidor.
        /// </summary>
        Announce,

        /// <summary>
        ///     Obtiene información de telemetría del servidor.
        /// </summary>
        Telemetry,

        /// <summary>
        ///     Inicia manualmente la ejecución de daemons.
        /// </summary>
        RunDaemons,

        /// <summary>
        ///     Indica que hay más bytes de comando extendido.
        /// </summary>
        Extension = byte.MaxValue
    }
}