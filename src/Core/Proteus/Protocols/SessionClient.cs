/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Models.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Networking;
using TheXDS.MCART.Networking.Client;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Protocols
{
    /// <summary>
    ///     Servicio de red que controla la sesión activa de Proteus.
    /// </summary>
    [Port(51200), Name("Cliente de arbitraje de sesión")]
    public class SessionClient :  ManagedCommandClient<Command, Response>
    {
        private readonly IDictionary<short, ResponseCallback> _extensions = new Dictionary<short, ResponseCallback>();
        private UdpClient _onlineListener;

        #region Constructores

        /// <summary>
        ///     Inicializa la clase <see cref="SessionClient"/>.
        /// </summary>
        static SessionClient()
        {
            ScanTypeOnCtor = false;
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="SessionClient"/>.
        /// </summary>
        public SessionClient()
        {
            WireUp(Response.Acknowledged, OnAcknowledged);
            WireUp(Response.Failure, OnFailure);
            WireUp(Response.Forbidden, OnForbidden);
            WireUp(Response.Identify, OnIdentify);
            WireUp(Response.EndSession, OnEndSession);
            WireUp(Response.ServiceRefresh, OnServiceRefresh);
            WireUp(Response.Alert, OnAlert);
            WireUp(Response.ImportantAlert, OnAlert);
            WireUp(Response.Probe, OnProbe);
            WireUp(Response.Extension, OnExtension);
        }

        internal void SetupListener()
        {
            if (!Proteus.Settings.EnableAnnounce) return;
            _onlineListener = new UdpClient(Proteus.Settings?.NetworkServerPort ?? 51200) { EnableBroadcast = true };
            AnnounceListen();
        }

        private async void AnnounceListen()
        {
            while (_onlineListener.Client.Connected)
            {
                var r = await _onlineListener.ReceiveAsync();
                if (!IsAlive)
                {
                    using var ms = new MemoryStream(r.Buffer);
                    using var br = new BinaryReader(ms);
                    var host = br.ReadString();
                    if (host != Proteus.Settings.NetworkServerAddress)
                    {
                        var p = Proteus.Settings.GetType().GetProperty(nameof(ISettings.NetworkServerAddress));
                        if (p.CanWrite)
                        {
                            p.SetValue(Proteus.Settings, host);
                        }
                        else
                        {
                            Proteus.MessageTarget?.Warning("Ha ocurrido un intento de Spoofing del servidor de sesión de red. Se detendrá el escucha de red por seguridad.");
                            return;
                        }
                    }
                    var lr = Proteus.Session is IUser iu ? new LoginResult(iu) : new LoginResult(LoginResultCode.NotLoggedIn);
                    var re = await Proteus.Connect(Task.FromResult(lr));
                    if (!re.Success)
                    {
                        Proteus.MessageTarget?.Warning($"Se intentó restablecer la conexión con el servidor de sesión {host}, pero ha devuelto un error: {re.Message}\n\rSe cerrará la sesión.");
                        Proteus.Logout();
                    }
                    else
                    {
                        var msj = $"Se ha restablecido la conectividad con el servidor de sesión {host}.";
                        if (Proteus.AlertTarget is null)
                        {
                            Proteus.MessageTarget?.Warning(msj);
                        }
                        else
                        {
                            Proteus.AlertTarget.Alert("Sesión de red", msj);
                        }
                    }
                }
            }
        }

        private void OnProbe(Response response, BinaryReader br)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Atención de respuestas

        private void OnExtension(Response response, BinaryReader br)
        {
            var r = br.ReadInt16();
            if (_extensions.ContainsKey(r))
            {
                _extensions[r]?.Invoke((Response)r, br);
            }
        }

        private void OnAcknowledged(Response response, BinaryReader br) { }

        private void OnFailure(Response response, BinaryReader br)
        {
            Proteus.MessageTarget?.Warning($"{response}: El servidor ha encontrado un error.");
            AbortCommands();
        }

        private void OnForbidden(Response response, BinaryReader br)
        {
            Proteus.MessageTarget?.Stop("El recurso no está disponible con las credenciales actuales.");
        }

        private void OnIdentify(Response response, BinaryReader br)
        {
            if (!IsAlive) return;

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            bw.Write(Environment.MachineName);
            bw.Write(Proteus.Session?.Id ?? string.Empty);

            //TODO: agregar soporte para canales
            bw.Write((short)0);

            bw.Flush();
            Send(Command.Register, ms, OnRegisterResponse);
        }

        private void OnEndSession(Response response, BinaryReader br)
        {
            if (!IsAlive) return;
            Send(Command.Unregister, (r, _) => Proteus.Logout());
        }

        private void OnServiceRefresh(Response response, BinaryReader br)
        {
            var s = br.ReadString();
            foreach (var j in Proteus.Services.OfType<IRefreshable>())
            {
                if (j.GetType().Name == s) j.Refresh();
            }
        }

        private void OnAlert(Response response, BinaryReader br)
        {
            Proteus.MessageTarget?.Info(br.ReadString());
        }

        #endregion

        #region Métodos privados

        private void OnRegisterResponse(Response response, BinaryReader b)
        {
            switch (response)
            {
                case Response.Acknowledged: return;
                case Response.Failure:
                    Proteus.MessageTarget?.Stop("No puede iniciar sesión en este equipo.");
                    break;

                case Response.Forbidden:
                    Proteus.MessageTarget?.Stop("Ya tiene otra sesión iniciada.");
                    break;
            }
            Proteus.Logout();
        }

        /// <summary>
        ///     Delegado que dedvuelve únicamente la respuesta de un diálogo
        ///     con el servidor.
        /// </summary>
        /// <param name="arg1">Respuesta enviada por el servidor.</param>
        /// <param name="_">
        ///     Lector de valores que acompañan a la respuesta.
        /// </param>
        /// <returns>
        ///     El valor de la respuesta enviada por el servidor.
        /// </returns>
        protected Response? ReturnResponse(Response arg1, BinaryReader _) => arg1;

        #endregion

        #region Funcionalidad

        /// <summary>
        ///     Intenta hacer inicio de sesión en el servidor de control de red.
        /// </summary>
        /// <param name="credential">Credencial de inicio de sesión.</param>
        /// <returns>
        ///     La respuesta enviada por el servidor.
        /// </returns>
        public Response TryLogin(IProteusCredential credential)
        {
            if (!IsAlive) return Response.Acknowledged;

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            bw.Write(Environment.MachineName);
            bw.Write(credential.Id);

            //TODO: agregar soporte para canales
            bw.Write((short)0);

            bw.Flush();
            return Send(Command.Register, ms, ReturnResponse) ?? Response.Failure;
        }

        /// <summary>
        ///     Ejecuta una operación de cierre de sesión en el servidor.
        /// </summary>
        internal void Logout()
        {
            if (Proteus.Settings?.EnableAnnounce ?? false)
            {
                _onlineListener?.Close();
            }

            if (!IsAlive) return;
            Send(Command.Unregister, (r, _) => CloseConnection());
        }

        /// <summary>
        ///     Envía un comando para actualizar el ViewModel especificado en
        ///     los demás clientes de red.
        /// </summary>
        /// <typeparam name="T">Tipo del ViewModel a actualizar.</typeparam>
        public void RefreshViewModel<T>()
        {
            if (!IsAlive) return;
            Send(Command.ViewModelRefresh, typeof(T).ResolveToDefinedType().Name);
        }

        /// <summary>
        ///  Envía un comando extendido al servidor.
        /// </summary>
        /// <param name="command">
        ///     Comando extendio a enviar.
        /// </param>
        /// <returns>
        ///     la respuesta enviada por el servidor.
        /// </returns>
        public Response SendExtended(short command)
        {
            if (!IsAlive) return Response.Acknowledged;
            return Send(Command.Extension, BitConverter.GetBytes(command), ReturnResponse) ?? Response.Failure;
        }

        /// <summary>
        ///  Envía un comando extendido al servidor.
        /// </summary>
        /// <param name="command">
        ///     Comando extendio a enviar.
        /// </param>
        /// <param name="data">
        ///     Datos adicionales que forman parte del comando.
        /// </param>
        /// <returns>
        ///     la respuesta enviada por el servidor.
        /// </returns>
        public Response SendExtended(short command, IEnumerable<byte> data)
        {
            if (!IsAlive) return Response.Acknowledged;
            return Send(Command.Extension, BitConverter.GetBytes(command).Concat(data), ReturnResponse) ?? Response.Failure;
        }

        /// <summary>
        ///  Envía un comando extendido al servidor.
        /// </summary>
        /// <param name="command">
        ///     Comando extendio a enviar.
        /// </param>
        /// <param name="data">
        ///     Datos adicionales que forman parte del comando.
        /// </param>
        /// <returns>
        ///     la respuesta enviada por el servidor.
        /// </returns>
        public Response SendExtended(short command, MemoryStream data)
        {
            if (!IsAlive) return Response.Acknowledged;
            return Send(Command.Extension, BitConverter.GetBytes(command).Concat(data.ToArray()), ReturnResponse) ?? Response.Failure;
        }

        /// <summary>
        ///  Envía un comando extendido al servidor.
        /// </summary>
        /// <param name="command">
        ///     Comando extendio a enviar.
        /// </param>
        /// <param name="data">
        ///     Datos adicionales que forman parte del comando.
        /// </param>
        /// <returns>
        ///     la respuesta enviada por el servidor.
        /// </returns>
        public Response SendExtended(short command, string data)
        {
            if (!IsAlive) return Response.Acknowledged;
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            bw.Write(command);
            bw.Write(data);
            return Send(Command.Extension, ms, ReturnResponse) ?? Response.Failure;
        }

        /// <summary>
        ///  Envía un comando extendido al servidor.
        /// </summary>
        /// <param name="command">
        ///     Comando extendio a enviar.
        /// </param>
        /// <param name="data">
        ///     Datos adicionales que forman parte del comando.
        /// </param>
        /// <returns>
        ///     la respuesta enviada por el servidor.
        /// </returns>
        public Response SendExtended(short command, IEnumerable<string> data)
        {
            if (!IsAlive) return Response.Acknowledged;
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            bw.Write(command);
            foreach (var j in data) bw.Write(j);
            return Send(Command.Extension, ms, ReturnResponse) ?? Response.Failure;
        }

        #endregion

        #region Configuración

        /// <summary>
        ///     Configura un comando extendido para atenderse por medio de este
        ///     cliente de red.
        /// </summary>
        /// <param name="response">
        ///     Respuesta a atender.
        /// </param>
        /// <param name="callback">
        ///     Método de atención.
        /// </param>
        [Thunk]
        public void Wire(Response response, ResponseCallback callback)
        {
            WireUp(response, callback);
        }

        /// <summary>
        ///     Configura un comando extendido para atenderse por medio de este
        ///     cliente de red.
        /// </summary>
        /// <param name="response">
        ///     Respuesta a atender.
        /// </param>
        /// <param name="callback">
        ///     Método de atención.
        /// </param>
        public void Extend(short response, ResponseCallback callback)
        {
            _extensions.Add(response, callback);
        }

        #endregion

    }
}