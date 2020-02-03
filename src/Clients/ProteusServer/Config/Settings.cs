/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Config
{
    internal sealed partial class Settings : ISettings
    {
        public Proteus.InitMode InitMode
        {
            get => (Proteus.InitMode)ProteusInitMode;
            set => ProteusInitMode = (byte)value;
        }

        public bool UseNetworkServer => false;

        public string NetworkServerAddress => string.Empty;

        public bool RequireNetworkServerSuccess => false;

        public bool EnableAnnounce => false;
    }
}