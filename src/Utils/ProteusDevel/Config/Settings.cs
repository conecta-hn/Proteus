/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Config {    
    internal sealed partial class Settings : ISettings
    {
        public bool UseNetworkServer => true;

        public Proteus.InitMode InitMode
        {
            get => (Proteus.InitMode)ProteusInitMode;
            set => ProteusInitMode = (byte)value;
        }

        public bool RequireNetworkServerSuccess => true;

        public bool EnableAnnounce => false;
    }
}
