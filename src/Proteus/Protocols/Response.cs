/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.Networking;

namespace TheXDS.Proteus.Protocols
{
    /// <summary>
    /// Códigos de respuesta del servidor.
    /// </summary>
    public enum Response : byte
    {
        /// <summary>
        /// Recibido/aceptado. Ninguna acción a proceder.
        /// </summary>
        Acknowledged,

        /// <summary>
        /// Operación fallida. Ninguna acción a proceder.
        /// </summary>
        [ErrorResponse] Failure,

        /// <summary>
        /// Operación restringida. Ninguna acción a proceder.
        /// </summary>
        Forbidden,

        /// <summary>
        /// Solicita una identificación.
        /// </summary>
        Identify,

        /// <summary>
        /// Solicita al cliente que cierre la sesión del usuario.
        /// </summary>
        EndSession,

        /// <summary>
        /// Solicita el refresco de un ViewModel
        /// </summary>
        ViewModelRefresh,

        /// <summary>
        /// Solicita que un servicio se refresque.
        /// </summary>
        ServiceRefresh,

        /// <summary>
        /// Solicita que una de sus páginas se refresque.
        /// </summary>
        PageRefresh,

        /// <summary>
        /// Solicita que se muestre una alerta.
        /// </summary>
        Alert,

        /// <summary>
        /// Solicita que se muestre una alerta importante.
        /// </summary>
        ImportantAlert,

        /// <summary>
        /// Query de datos de los usuarios.
        /// </summary>
        ClientQuery,

        /// <summary>
        /// Query de la sesión.
        /// </summary>
        SessionQuery,

        /// <summary>
        /// Notificar de un recurso libre.
        /// </summary>
        Notify,

        /// <summary>
        /// Reenvía el timestamp enviado por el servidor.
        /// </summary>
        Probe,

        /// <summary>
        /// Obtiene un listado de objetos abiertos para edición.
        /// </summary>
        QueryLocks,

        /// <summary>
        /// Indica que hay más bytes de respuesta extendida.
        /// </summary>
        Extension = byte.MaxValue
    }
}