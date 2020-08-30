/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using System;
using System.IO;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Events;
using TheXDS.MCART.Networking.Legacy;
using TheXDS.MCART.Networking.Legacy.Server;
using static TheXDS.MCART.Types.Extensions.EnumExtensions;

namespace TheXDS.Proteus.Protocols
{
    [Port(51201), Name("Servicio de terminal de salida remota")]
    public class RemoteDumpProtocol : ManagedCommandProtocol<Client<bool>, DumpCommand, DumpResponse>, IProteusProtocol<Client<bool>>, IMessageTarget, IStatusReporter
    {
        public bool IsBusy { get; private set; }

        static RemoteDumpProtocol()
        {
            ScanTypeOnCtor = false;
        }

        private void Send(DumpResponse resp, string message)
        {
            byte[] pkt1, pkt2;

            // Paquete mejorado
            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.Write(MakeResponse(resp));
                bw.Write(message);
                pkt1 = ms.ToArray();
            }

            // Paquete compatible con Telnet
            using (var ms = new MemoryStream())
            {
                ms.WriteByte(0);
                ms.WriteByte((byte)DumpResponse.Message);
                var m = System.Text.Encoding.ASCII.GetBytes($" {resp.NameOf()}: {message}".PadRight(33));
                ms.Write(m, 0, m.Length);
                ms.WriteByte(13);
                ms.WriteByte(10);
                pkt2 = ms.ToArray();
            }

            Server.MulticastAsync(pkt1, p => p.ClientData);
            Server.MulticastAsync(pkt2, p => !p.ClientData);
        }

        private void Rprt(double progress, string message)
        {
            byte[] pkt1, pkt2;

            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                ms.WriteByte(0);
                bw.Write(MakeResponse(DumpResponse.Report));
                bw.Write(progress);
                bw.Write(message);
                pkt1 = ms.ToArray();
            }
            using (var ms = new MemoryStream())
            {
                ms.WriteByte((byte)DumpResponse.Message);
                var m = System.Text.Encoding.ASCII.GetBytes((double.IsNaN(progress) ? $" {message}...":$" {message}...{progress}%").PadRight(32));
                ms.Write(m, 0, m.Length);
                ms.WriteByte(13);
                ms.WriteByte(10);
                pkt2 = ms.ToArray();
            }

            Server.MulticastAsync(pkt1, p => p.ClientData);
            Server.MulticastAsync(pkt2, p => !p.ClientData);
        }

        public RemoteDumpProtocol()
        {
            AppendGuid = false;
            CompoundTarget.Register(this);
            WireUp(DumpCommand.Disconnect, r => r.Client.Bye());
            WireUp(DumpCommand.Enhance, r => r.Client.ClientData = true);
            ServerError += OnServerError;
        }

        public override bool ClientWelcome(Client<bool> client)
        {
            Proteus.MessageTarget?.Info($"Se ha adjuntado una terminal remota desde {client.EndPoint}");
            return base.ClientWelcome(client);
        }
        public override void ClientBye(Client<bool> client)
        {
            Proteus.MessageTarget?.Info($"Se ha desconectado una terminal remota desde {client.EndPoint}");
            base.ClientBye(client);
        }
        public override void ClientDisconnect(Client<bool> client)
        {
            Proteus.MessageTarget?.Info($"Se ha desconectado una terminal remota desde {client.EndPoint}");
            base.ClientDisconnect(client);
        }

        private void OnServerError(object? sender, ExceptionEventArgs e)
        {
            try
            {
                Critical(e.Value ?? new Exception("Error desconocido."));
            }
            catch { }
        }

        public Server<Client<bool>> BuildServer() => new Server<Client<bool>>(this);

        public void Critical(string message)
        {
            Send(DumpResponse.Critical, message);
        }

        public void Critical(Exception ex)
        {
            Send(DumpResponse.Critical, ex.Message);
        }

        public void Done()
        {
            IsBusy = false;
            Done("Operación finalizada correctamente.");
        }

        public void Done(string text)
        {
            IsBusy = false;
            Send(DumpResponse.Done, text);
        }

        public void Error(string message)
        {
            Send(DumpResponse.Error, message);
        }

        public void Info(string message)
        {
            Send(DumpResponse.Info, message);
        }

        public void Show(string message)
        {
            Send(DumpResponse.Message, message);
        }

        public void Show(string title, string message)
        {
            Send(DumpResponse.Message, $"{title}:\n{message}");
        }

        public void Stop(string message)
        {
            Send(DumpResponse.Stop, message);
        }

        public void UpdateStatus(double progress)
        {
            IsBusy = true;
            Rprt(progress, "Realizando operación secundaria");
        }

        public void UpdateStatus(double progress, string text)
        {
            IsBusy = true;
            Rprt(progress, text);
        }

        public void UpdateStatus(string text)
        {
            IsBusy = true;
            Rprt(double.NaN, text);
        }

        public void Warning(string message)
        {
            Send(DumpResponse.Warning, message);
        }
    }
}