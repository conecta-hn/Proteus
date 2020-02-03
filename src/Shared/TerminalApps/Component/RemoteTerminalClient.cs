/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.IO;
using TheXDS.MCART.Networking.Client;

namespace TheXDS.Proteus.Component
{
    public class RemoteTerminalClient : ManagedCommandClient<DumpCommand, DumpResponse>
    {
        private readonly IMessageTarget _target;
        private readonly IStatusReporter _reporter;
        public RemoteTerminalClient(IMessageTarget target, IStatusReporter reporter)
        {
            _target = target;
            _reporter = reporter;
            WireUp(DumpResponse.Critical, OnPrintMessage);
            WireUp(DumpResponse.Done, OnPrintMessage);
            WireUp(DumpResponse.Error, OnPrintMessage);
            WireUp(DumpResponse.Info, OnPrintMessage);
            WireUp(DumpResponse.Message, OnPrintMessage);
            WireUp(DumpResponse.Stop, OnPrintMessage);
            WireUp(DumpResponse.Report, OnPrintMessage);
            WireUp(DumpResponse.Warning, OnPrintMessage);
        }

        public void Enhance()
        {
            Send(DumpCommand.Enhance);
        }

        private void OnPrintMessage(DumpResponse response, BinaryReader br)
        {
            switch (response)
            {
                case DumpResponse.Critical:
                    _target.Critical(br.ReadString());
                    break;
                case DumpResponse.Done:
                    _reporter.Done(br.ReadString());
                    break;
                case DumpResponse.Error:
                    _target.Error(br.ReadString()); break;
                case DumpResponse.Info:
                    _target.Info(br.ReadString()); break;
                case DumpResponse.Message:
                    _target.Show(br.ReadString()); break;
                case DumpResponse.Stop:
                    _target.Stop(br.ReadString()); break;
                case DumpResponse.Report:
                    _reporter.UpdateStatus(br.ReadDouble(),br.ReadString()); break;
                case DumpResponse.Warning:
                    _target.Warning(br.ReadString()); break;
            }
        }
    }
}