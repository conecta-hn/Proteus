/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TheXDS.MCART;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Events;
using TheXDS.MCART.Networking;
using TheXDS.MCART.Networking.Server;
using TheXDS.MCART.Types.Extensions;
using static TheXDS.Proteus.Protocols.Command;
using static TheXDS.Proteus.Protocols.Response;
using static TheXDS.MCART.Types.Extensions.StringExtensions;

namespace TheXDS.Proteus.Protocols
{
    /// <inheritdoc cref="SelfWiredCommandProtocol{TClient,TCommand,TResponse}" />
    /// <summary>
    /// Protocolo de arbitraje de sesión y bloqueo de recursos de Proteus.
    /// </summary>
    [Port(51200), Name("Servicio de control de red")]
    public class SessionProtocol : ManagedCommandProtocol<Client<HostData>, Command, Response>, IProteusProtocol, IAnnounceAvailability
    {
        private readonly IDictionary<short, CommandCallback> _extensions = new Dictionary<short, CommandCallback>();
        private bool _deaf = false;

        /// <summary>
        ///     Construye un nuevo servidor qpara alojar este protocolo.
        /// </summary>
        /// <returns></returns>
        public Server BuildServer() => new Server<Client<HostData>>(this);

        static SessionProtocol()
        {
            ScanTypeOnCtor = false;
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase 
        ///     <see cref="SessionProtocol"/>.
        /// </summary>
        public SessionProtocol()
        {
            WireUp(Heartbeat, OnHeartbeat);
            WireUp(Register, OnRegister);
            WireUp(Unregister, OnUnregister);
            WireUp(Check, OnCheck);
            WireUp(Lock, OnLock);
            WireUp(Unlock, OnUnlock);
            WireUp(Command.ServiceRefresh, OnServiceRefresh);
            WireUp(Command.PageRefresh, OnPageRefresh);
            WireUp(Command.ViewModelRefresh, OnViewModelRefresh);
            WireUp(QueryUsers, OnQueryUsers);
            WireUp(QuerySession, OnQuerySession);
            WireUp(AlertTo, OnAlertTo);
            WireUp(Purge, OnPurge);
            WireUp(Enumerate, OnEnumerate);
            WireUp(Close, OnClose);
            WireUp(Deaf, OnDeaf);
            WireUp(Command.Announce, OnAnnounce);
            WireUp(Listen, OnListen);
            WireUp(Shutdown, OnShutdown);
            WireUp(Command.Probe, OnProbe);
            WireUp(Telemetry, OnTelemetry);
            WireUp(RunDaemons, OnRunDaemons);
            WireUp(Command.Extension, OnExtension);

            ServerError += SessionProtocol_ServerError;

            foreach (var j in Objects.FindAllObjects<ProteusProtocolExtensionPlugin>())
            {
                j.WireUp(this);
            }
        }

        private void OnRunDaemons(Request request)
        {
            request.Respond(Acknowledged);
            Program._service.RunDaemons();
        }

        private void OnTelemetry(Request request)
        {
            var v = new TheXDS.MCART.Component.WindowsInfo();
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write(Environment.MachineName);
            bw.Write(v.Caption);
            bw.Write(v.Version.Major);
            bw.Write(v.Version.Minor);
            bw.Write(v.Version.Build);
            bw.Write(v.Version.Revision);

            request.Respond(Acknowledged, ms);
        }

        private void SessionProtocol_ServerError(object sender, ExceptionEventArgs e)
        {
            Proteus.MessageTarget?.Error($"El servidor ha encontrado un error interno. (¿Posible comando malformado?)\n{e.Value.Message}\n----------\n{e.Value.StackTrace}");
        }

        #region Funciones de protocolo

        /// <summary>
        ///     Atiende la desconexión controlada de un cliente.
        /// </summary>
        /// <param name="client">
        ///     Cliente que solicita la desconexión.
        /// </param>
        public override void ClientBye(Client<HostData> client)
        {
            Proteus.MessageTarget?.Show($"{client.ClientData.UserId} ha cerrado sesión en {client.ClientData.HostId}");
            NotifyFree(client);
            client.ClientData = null;
            base.ClientBye(client);
        }

        /// <summary>
        ///     Atiende la desconexión inesperada de un cliente.
        /// </summary>
        /// <param name="client">
        ///     Cliente desconectado inesperadamente.
        /// </param>
        public override void ClientDisconnect(Client<HostData> client)
        {
            Proteus.MessageTarget?.Warning($"Se ha perdido la conexión con el host {client.ClientData?.HostId ?? client.EndPoint.ToString()}{client.ClientData?.UserId.OrNull(", {0} desconectado")}.");
            NotifyFree(client);
            client.ClientData = null;
            base.ClientDisconnect(client);
        }

        /// <summary>
        ///     Anuncia la disponibilidad de este servicio en la red.
        /// </summary>
        public void Announce() => OnAnnounce(null);
        #endregion

        #region Métodos auxiliares

        private void NotifyFree(Client<HostData> client)
        {
            var sw = new StringBuilder();
            if (!client.ClientData?.Locks.Any() ?? true) return;
            sw.AppendLine($"Liberación automática de recursos bloqueados por {client.ClientData.UserId}");
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.Write(client.ClientData.Locks.Count);
                foreach (var j in client.ClientData.Locks)
                {
                    sw.AppendLine($"  - {j.Kind} '{j.Id}'");
                    bw.Write(j.Kind);
                    bw.Write(j.Id);
                }
                Server.Broadcast(new[] { (byte)0, (byte)Notify }.Concat(ms.ToArray()).ToArray(), client);
            }
            Proteus.MessageTarget?.Show(sw.ToString());
        }
        private bool AnyLocks(Request request, out string kind, out string id)
        {
            var k = request.Reader.ReadString(); kind = k;
            var i = request.Reader.ReadString(); id = i;
            return request.Server.Clients.SelectMany(p => p.ClientData?.Locks).Any(j => j.Kind == k && j.Id == i);
        }
        private bool FailsLogin(Request request)
        {
            if (request.Client.ClientData?.UserId.IsEmpty() ?? false)
            {
                request.Respond(Forbidden);
                Proteus.MessageTarget?.Stop($"El cliente {request.Client.EndPoint} ha intentado realizar una operación sin inciar sesión. Se ha denegado la solicitud.");
                return true;
            }
            return false;
        }
        private bool FailsAdmin(Request request)
        {
            var k = request.Client.ClientData.Kind;
            if (k == HostData.SessionKind.Regular)
            {
                request.Respond(Forbidden);
                Proteus.MessageTarget?.Stop($"El cliente {request.Client.ClientData} ha intentado realizar una operación administrativa sin contar con los permisos necesarios. Se ha denegado la solicitud.");
                return true;
            }
            return false;
        }
        private void OnRefresh(Request request, Response resp)
        {
            if (FailsLogin(request)) return;
            request.Respond(Acknowledged);
            var obj = request.Reader.ReadString();
            request.Broadcast(resp, obj);
            Proteus.MessageTarget?.Info($"{resp.NameOf()} {obj} iniciado desde en {request.Client.ClientData.HostId}");

        }

        #endregion

        #region Comandos del protocolo

        private void OnProbe(Request request)
        {
            if (FailsLogin(request) || FailsAdmin(request)) return;
            var index = request.Reader.ReadInt16();
            Client<HostData> client;
            if (request.Server.Clients is IList<Client<HostData>> l)
            {
                client = l[index];
            }
            else
            {
                var en = request.Server.Clients.GetEnumerator();
                for (var j = 0; j < index; j++)
                {
                    if (!en.MoveNext())
                    {
                        request.Respond(Failure);
                        return;
                    }
                }
                client = en.Current;
            }
            request.Respond(Acknowledged);

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            var gd = Guid.NewGuid();
            bw.Write(gd.ToByteArray());
            var ts = DateTime.Now;
            bw.Write(ts.ToBinary());




            client.Send(MakeResponse(Response.Probe).Concat(ms.ToArray()).ToArray());

        }
        private void OnShutdown(Request request)
        {
            if (FailsLogin(request) || FailsAdmin(request)) return;
            Proteus.MessageTarget?.Warning($"El cliente {request.Client.ClientData} ha realizado una solicitud de detención remota del servidor.");
            request.Respond(EndSession);
            request.Broadcast(EndSession);
            Server.Stop();
            Program.Exit();
        }
        private void OnListen(Request request)
        {
            if (FailsLogin(request) || FailsAdmin(request)) return;
            Proteus.MessageTarget?.Info($"El servidor estará atento a las nuevas conexiones entrantes.");
            _deaf = false;
            request.Respond(Acknowledged);
        }
        private void OnDeaf(Request request)
        {
            if (FailsLogin(request) || FailsAdmin(request)) return;
            Proteus.MessageTarget?.Warning($"El servidor estará 'sordo' a las nuevas conexiones entrantes.");
            _deaf = true;
            request.Respond(Acknowledged);
        }
        private void OnAnnounce(Request request)
        {
            /* En este caso, request podría ser null cuando se llama desde otro
             * punto de la aplicación (sin atender un comando) */
            if (!(request is null))
            {
                if (FailsLogin(request) || FailsAdmin(request)) return;
                request.Respond(Acknowledged);
            }
            var ep = Server.ListeningEndPoint;
            var addr = new IPEndPoint(IPAddress.Broadcast, ep.Port);
            using var c = new UdpClient() { EnableBroadcast = true };
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            bw.Write(Environment.MachineName);
            var d = ms.ToArray();
            c.Send(d, d.Length, addr);
            Proteus.MessageTarget?.Info($"Se ha anunciado la disponibilidad del servicio de red por medio de UDP {ep.Port} al puerto TCP {Server.ListeningEndPoint.Port}");
        }
        private void OnClose(Request request)
        {
            if (FailsLogin(request) || FailsAdmin(request)) return;
            var index = request.Reader.ReadInt16();
            if (request.Server.Clients is IList<Client<HostData>> l)
            {
                var c = l[index];
                Proteus.MessageTarget?.Warning($"Cierre forzoso de conexión con {c?.ClientData.ToString() ?? c?.EndPoint?.ToString() ?? "???"}");
                c?.Disconnect();
            }
            else
            {
                var en = request.Server.Clients.GetEnumerator();
                for (var j = 0; j < index; j++)
                {
                    if (!en.MoveNext())
                    {
                        request.Respond(Failure);
                        return;
                    }
                }
                Proteus.MessageTarget?.Warning($"Cierre forzoso de conexión con {en.Current?.ClientData?.ToString() ?? en.Current?.EndPoint?.ToString() ?? "???"}");
                en.Current?.Disconnect();
            }
            request.Respond(Acknowledged);
        }
        private void OnEnumerate(Request request)
        {
            if (FailsLogin(request) || FailsAdmin(request)) return;
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write((short)request.Server.Clients.Count());
            foreach (var j in request.Server.Clients)
            {
                bw.Write(j.EndPoint.Address.GetAddressBytes());
                bw.Write(j.EndPoint.Port);
                bw.Write(j.ClientData.HostId);
                bw.Write(j.ClientData.UserId);
                bw.Write((byte)j.ClientData.Kind);
            }
            request.Respond(Acknowledged, ms);
        }
        private void OnExtension(Request request)
        {
            var r = request.Reader.ReadInt16();
            if (_extensions.ContainsKey(r))
            {
                _extensions[r]?.Invoke(request);
            }
        }
        private void OnHeartbeat(Request request)
        {
            Proteus.MessageTarget?.Show("<3", $"Heartbeat {DateTime.Now.ToBinary()}");
            request.Respond(Acknowledged);
        }
        private void OnRegister(Request request)
        {
            if (_deaf)
            {
                Proteus.MessageTarget?.Stop($"Se ha denegado el inicio de sesión a {request.Client.EndPoint} debido a que el servidor está 'sordo'.");
                request.Respond(Failure);
                return;
            }

            var hostId = request.Reader.ReadString();
            var userId = request.Reader.ReadString();
            var mcCount = request.Reader.ReadInt16();

            var u = Proteus.Service<UserService>().Get<User, string>(userId);

            if (u is null || !u.AllowMultiLogin && request.Server.Clients.Any(p => p.ClientData?.UserId == userId))
            {
                request.Respond(Forbidden);
                return;
            }

            request.Client.ClientData = new HostData(hostId, userId, !u.Interactive ? HostData.SessionKind.Service : u.DefaultGranted == SecurityFlags.Root ? HostData.SessionKind.Admin : HostData.SessionKind.Regular);

            while (mcCount-- > 0)
            {
                request.Client.ClientData.MulticastChannels.Add(request.Reader.ReadString());
            }

            Proteus.MessageTarget?.Show($"Inicio de sesión aceptado desde {hostId} ({request.Client.EndPoint}): {userId}");

            request.Respond(Acknowledged);
        }
        private void OnUnregister(Request request)
        {
            if (FailsLogin(request)) return;
            request.Respond(Acknowledged);
            request.Client.Bye();
        }
        private void OnCheck(Request request)
        {
            if (FailsLogin(request)) return;
            request.Respond(AnyLocks(request, out _, out _) ? Forbidden : Acknowledged);
        }
        private void OnLock(Request request)
        {
            if (FailsLogin(request)) return;

            if (AnyLocks(request, out var kind, out var id))
            {
                request.Failure();
            }
            else
            {
                Proteus.MessageTarget?.Info($"El {kind} '{id}' ha sido bloqueado por {request.Client.ClientData.UserId} en {request.Client.ClientData.HostId}");
                request.Client.ClientData.Locks.Add(new ResourceLock(kind, id));
                request.Respond(Acknowledged);
            }
        }
        private void OnUnlock(Request request)
        {
            if (FailsLogin(request)) return;
            var kind = request.Reader.ReadString();
            var id = request.Reader.ReadString();
            var l = request.Client.ClientData.Locks.FirstOrDefault(p => p.Kind == kind && p.Id == id);

            if (l is null)
            {
                request.Failure();
            }
            else
            {
                Proteus.MessageTarget?.Info($"El {kind} '{id}' bloqueado por {request.Client.ClientData.UserId} en {request.Client.ClientData.HostId} ha sido liberado.");
                request.Client.ClientData.Locks.Remove(l);
                request.Respond(Acknowledged);
            }
        }
        private void OnServiceRefresh(Request request) => OnRefresh(request, Response.ServiceRefresh);
        private void OnPageRefresh(Request request) => OnRefresh(request, Response.PageRefresh);
        private void OnViewModelRefresh(Request request) => OnRefresh(request, Response.ViewModelRefresh);
        private void OnQueryUsers(Request request)
        {
            if (FailsLogin(request) || FailsAdmin(request)) return;
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            bw.Write(request.Server.Clients.Count());
            foreach (var j in request.Server.Clients)
            {
                bw.Write(j.ClientData.HostId);
                bw.Write(j.ClientData.UserId);
                bw.Write((byte)j.ClientData.Kind);
            }
            request.Respond(ClientQuery, ms);
        }
        private void OnQuerySession(Request request)
        {
            if (FailsLogin(request) || FailsAdmin(request)) return;
            var queryId = request.Reader.ReadBoolean() ? request.Reader.ReadString() : request.Client.ClientData.UserId;
            var sessions = request.Server.Clients.Select(p => p.ClientData).Where(p => p.UserId == queryId).ToArray();
            request.Respond(SessionQuery, sessions.Select(p => p.HostId));
        }
        private void OnAlertTo(Request request)
        {
            if (FailsLogin(request)) return;

            var message = request.Reader.ReadString();
            var channelCount = request.Reader.ReadInt16();
            var channels = new HashSet<(string channel, bool important)>();

            if (channelCount > 0)
            {
                do
                {
                    channels.Add((request.Reader.ReadString(), request.Reader.ReadBoolean()));
                } while (--channelCount > 0);

                foreach (var (channel, important) in channels)
                {
                    request.Multicast(important ? ImportantAlert : Alert, message,
                        p => p.ClientData.MulticastChannels.Contains(channel) || p.ClientData.UserId == channel);
                }
            }
            else
            {
                request.Broadcast(request.Reader.ReadBoolean() ? ImportantAlert : Alert, message);
            }
            request.Respond(Acknowledged);
        }

        public void SendAlert(string message)
        {            
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            bw.Write(false);
            bw.Write(Alert.ToBytes());
            bw.Write(message);
            Server.Broadcast(ms.ToArray());
        }

        private void OnPurge(Request request)
        {
            if (FailsLogin(request) || FailsAdmin(request)) return;
            var c = request.Reader.ReadInt16();
            if (c > 0)
            {
                do
                {
                    var u = request.Reader.ReadString();
                    Proteus.MessageTarget?.Info($"Expulsando a {u} de todas las sesiones activas...");
                    request.Multicast(EndSession, p => p.ClientData.UserId == u);
                } while (--c > 0);
            }
            else
            {
                Proteus.MessageTarget?.Warning($"El cliente {request.Client.ClientData} ha realizado una solicitud de expulsión de todas las demás sesiones activas.");
                request.Broadcast(EndSession);
            }
            request.Respond(Acknowledged);
        }

        #endregion

        #region Configuración

        /// <summary>
        ///     Configura un comando extendido para atenderse por medio de este
        ///     cliente de red.
        /// </summary>
        /// <param name="command">
        ///     Respuesta a atender.
        /// </param>
        /// <param name="callback">
        ///     Método de atención.
        /// </param>
        [Thunk]
        public void Wire(Command command, Action<Request> callback)
        {
            WireUp(command, callback.Method.ToDelegate<CommandCallback>());
        }

        /// <summary>
        ///     Configura un comando extendido para atenderse por medio de este
        ///     cliente de red.
        /// </summary>
        /// <param name="command">
        ///     Respuesta a atender.
        /// </param>
        /// <param name="callback">
        ///     Método de atención.
        /// </param>
        public void Extend(short command, Action<Request> callback)
        {
            _extensions.Add(command, callback.Method.ToDelegate<CommandCallback>());
        }

        /// <summary>
        ///     Instruye al protocolo de reenviar el mismo paquete de comando
        ///     extendido a todos los clientes conectados.
        /// </summary>
        /// <param name="command">
        ///     Comando extendido a registrar para el reenvío de datos.
        /// </param>
        public void Relay(short command)
        {
            Extend(command, request =>
            {
                request.Respond(Acknowledged);
                var ms = request.Reader.BaseStream as MemoryStream;
                var toEnd = (int)(ms.Length - ms.Position);
                var data = BitConverter.GetBytes(command).Concat(request.Reader.ReadBytes(toEnd));
                request.Broadcast(Response.Extension, data);
            });
        }

        #endregion
    }
}