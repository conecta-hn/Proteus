/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using Microsoft.Extensions.Configuration;

namespace TheXDS.Proteus.Config
{
    public class ProteusSettings : ISettings
    {
        internal IConfigurationSection settings { get; set; }
        public byte ProteusInitMode { get; set; }

        public Proteus.InitMode InitMode
        {
            get => (Proteus.InitMode)ProteusInitMode;
            set => ProteusInitMode = (byte)value;
        }

        public bool UseDomainProvider { get; set; }

        public string DomainProvider { get; set; }

        public bool UseLocalDbProvider { get; set; }

        public bool UseCustomProvider { get; set; }

        public string CustomProvider { get; set; }

        public bool UseNetworkServer { get; set; }

        public string NetworkServerAddress { get; set; }

        public int NetworkServerPort { get; set; }

        public string PluginsDir { get; set; }

        /// <summary>
        /// Contiene la ruta al directorio de almacenamiento local del host. El
        /// directorio debería tener permisos de escritura habilitados.
        /// </summary>
        public string DataDir { get; set; }

        public string WebPluginsDir { get; set; }

        public int ServerTimeout { get; set; }

        public bool RequireNetworkServerSuccess { get; set; }

        public bool CheckExists { get; set; }
        public string ServiceApiToken { get; set; }

        public bool EnableAnnounce { get; set; }

        public string BusinessName { get; set; }

        public int RowLimit { get; set; }
    }
}
