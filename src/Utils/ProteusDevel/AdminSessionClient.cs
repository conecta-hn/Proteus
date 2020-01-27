/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Networking;
using TheXDS.Proteus.Protocols;

namespace TheXDS.Proteus.Devel
{
    /// <summary>
    /// Estructura que contiene información sobre el estado de una conexión
    /// devuelto por el servicio de arbitraje de sesión.
    /// </summary>
    public struct Connection
    {
        public IPAddress Address { get; }
        public int Port { get; }
        public string UserId { get; }
        public string HostId { get; }
        public SessionKind Kind { get; }
        public Connection(BinaryReader br)
        {
            Address = new IPAddress(br.ReadBytes(4));
            Port = br.ReadInt32();
            UserId = br.ReadString();
            HostId = br.ReadString();
            Kind = (SessionKind)br.ReadByte();
        }
    }

    public struct UserQuery
    {
        public string Host { get; }
        public string User { get; }
        public SessionKind Kind { get; }
        public UserQuery(BinaryReader br)
        {
            Host = br.ReadString();
            User = br.ReadString();
            Kind = (SessionKind)br.ReadByte();
        }
    }

    /// <summary>
    ///     Enumera los distintos tipos de sesión que pueden existir en el
    ///     servidor.
    /// </summary>
    public enum SessionKind : byte
    {
        /// <summary>
        ///     Sesión regular.
        /// </summary>
        [Name("Sesión de cliente")] Regular,

        /// <summary>
        ///     Sesión para un servicio no interactivo.
        /// </summary>
        [Name("Servicio")] Service,

        /// <summary>
        ///     Sesión con facultades administrativas.
        /// </summary>
        [Name("Sesión administrativa")] Admin
    }

    /// <summary>
    ///     Cliente extendido que ofrece acceso a comandos administrativos del servidor.
    /// </summary>
    [Port(51200), Name("Cliente administrativo de arbitraje de sesión")]
    public class AdminSessionClient : SessionClient
    {
        /// <summary>
        ///     Obtiene un listado de las sesiones activas.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserQuery> QueryUsers()
        {
            return Send(Command.QueryUsers, (resp, br) =>
            {
                var l = new List<UserQuery>();
                if (resp == Response.ClientQuery)
                {
                    var c = br.ReadInt32();
                    for (var j = 0; j < c; j++)
                    {
                        l.Add(new UserQuery(br));
                    }
                }
                return l;
            });
        }
        public IEnumerable<Connection> EnumerateConnections()
        {
            return Send(Command.Enumerate, OnEnumResp);
        }

        private IEnumerable<Connection> OnEnumResp(Response arg1, BinaryReader arg2)
        {
            if (arg1 != Response.Acknowledged) yield break;
            var c = arg2.ReadInt16();
            while(c-- > 0)
            {
                yield return new Connection(arg2);
            }
        }

        /// <summary>
        ///     Determina la salud del servidor enviando un paquete de latido
        ///     de corazón.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> si el servidor está activo y saludable,
        ///     <see langword="false"/> en caso contrario.
        /// </returns>
        public bool SendHeartbeat()
        {
            return IsAlive && (Send(Command.Heartbeat, ReturnResponse) == Response.Acknowledged);
        }

        public void SendAlert(string msg, IEnumerable<string> channels)
        {            
            SendAlert(msg, channels, false);
        }

        public void SendAlert(string msg, IEnumerable<string> channels, bool important)
        {
            var l = channels.ToList();
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            bw.Write(msg);
            bw.Write((short)l.Count);
            bw.Write(important);
            foreach (var j in l)
            {
                bw.Write(j);
            }
            Send(Command.AlertTo, ms);
        }

        public void SendPurge() => SendPurge(System.Array.Empty<string>());

        public void SendPurge(IEnumerable<string> users)
        {
            var l = users.ToList();
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            bw.Write((short)l.Count);
            foreach (var j in l)
            {
                bw.Write(j);
            }
            Send(Command.Purge, ms);
        }

        public void SendShutdown()
        {
            Send(Command.Shutdown);
        }

        public void SendRunDaemons(IEnumerable<string> daemons)
        {
            var c = daemons.ToList();
            if (c.Count > short.MaxValue) throw new TheXDS.MCART.Exceptions.TooManyArgumentsException();
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            bw.Write((short)c.Count);
            foreach (var j in c)
            {
                bw.Write(j);
            }
            Send(Command.RunDaemons, ms);
        }

        public void SendListen()
        {
            Send(Command.Listen);
        }

        public void SendDeaf()
        {
            Send(Command.Deaf);
        }

        public void SendAnnounce()
        {
            Send(Command.Announce);
        }

        public void SendClose(short index)
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            bw.Write(index);
            Send(Command.Close, ms);
        }
    }
}