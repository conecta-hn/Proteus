/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Protocols
{
    /// <summary>
    ///     Clase que contiene información sobre los clientes que hayan
    ///     iniciado sesión.
    /// </summary>
    public class HostData
    {
        /// <summary>
        ///     Enumera los distintos tipos de sesión que pueden existir en el
        ///     servidor.
        /// </summary>
        public enum SessionKind : byte
        {
            /// <summary>
            ///     Sesión regular.
            /// </summary>
            Regular,

            /// <summary>
            ///     Sesión para un servicio no interactivo.
            /// </summary>
            Service,

            /// <summary>
            ///     Sesión con facultades administrativas.
            /// </summary>
            Admin
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="HostData"/>.
        /// </summary>
        /// <param name="hostId">Id del equipo que inicia sesión.</param>
        /// <param name="userId">Id del usuario que inicia sesión.</param>
        /// <param name="kind">Tipo de sesión creada.</param>
        public HostData(string hostId, string userId, SessionKind kind)
        {
            HostId = hostId.OrNull() ?? throw new ArgumentNullException(nameof(hostId));
            UserId = userId.OrNull() ?? throw new ArgumentNullException(nameof(userId));
            Kind = kind;
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="HostData"/>.
        /// </summary>
        /// <param name="hostId">Id del equipo que inicia sesión.</param>
        /// <param name="userId">Id del usuario que inicia sesión.</param>
        public HostData(string hostId, string userId):this(hostId, userId, SessionKind.Regular)
        { }

        /// <summary>
        ///     Identificador del host.
        /// </summary>
        public string HostId { get; }

        /// <summary>
        ///     Identificador del usuario de la sesión.
        /// </summary>
        public string UserId { get; }

        /// <summary>
        ///     Tipo de sesión asociada al cliente.
        /// </summary>
        public SessionKind Kind { get; }

        /// <summary>
        ///     Grupo de pertenencia para Multi-cast
        /// </summary>
        public HashSet<string> MulticastChannels { get; } = new HashSet<string>();

        /// <summary>
        ///     Objetos bloqueados por el cliente.
        /// </summary>
        public HashSet<ResourceLock> Locks { get; } = new HashSet<ResourceLock>();

        public override string ToString()
        {
            return $"{UserId} en {HostId}";
        }
    }

    /// <summary>
    ///     Contiene información sobre los recursos bloqueados por un cliente.
    /// </summary>
    public class ResourceLock
    {
        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ResourceLock"/>.
        /// </summary>
        /// <param name="kind">
        ///     Identificador del tipo de elemento que ha sido bloqueado.
        /// </param>
        /// <param name="id">
        ///     Id del objeto bloqueado.
        /// </param>
        public ResourceLock(string kind, string id)
        {
            Kind = kind.OrNull() ?? throw new ArgumentNullException(nameof(kind));
            Id = id.OrNull() ?? throw new ArgumentNullException(nameof(id));
        }

        /// <summary>
        ///     Identifica el tipo de elemento bloqueado representado por este
        ///     <see cref="ResourceLock"/>.
        /// </summary>
        public string Kind { get; }

        /// <summary>
        ///     Id del elemento bloqueado representado por este
        ///     <see cref="ResourceLock"/>.
        /// </summary>
        public string Id { get; }
    }
}