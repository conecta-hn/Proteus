/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace TheXDS.Proteus.Component
{
    /// <inheritdoc />
    /// <summary>
    /// Establece la configuración activa de servidor de base de datos.
    /// </summary>
    public class DbConfig : DbConfiguration
    {
        private class FactoryRegistryEntry
        {
            public Func<bool> Predicate { get; }
            public Func<IDbConnectionFactory> Factory { get; }
            public FactoryRegistryEntry(Func<bool> predicate, Func<IDbConnectionFactory> factory)
            {
                Predicate = predicate;
                Factory = factory;
            }
        }

        internal static bool _forceLocalDb;
        private static readonly List<FactoryRegistryEntry> _factories = new List<FactoryRegistryEntry>();

        /// <summary>
        /// Inicializa la clase <see cref="DbConfig"/>
        /// </summary>
        static DbConfig()
        {
            _factories.Add(new FactoryRegistryEntry(
                () => Proteus.Settings?.UseLocalDbProvider ?? true,
                () => new LocalDbConnectionFactory("mssqllocaldb")));

            _factories.Add(new FactoryRegistryEntry(
                () => Proteus.Settings?.UseDomainProvider ?? false,
                () => new DomainProviderFactory(Proteus.Settings!.DomainProvider)));

            _factories.Add(new FactoryRegistryEntry(
                () => Proteus.Settings?.UseCustomProvider ?? false,
                () => new SqlConnectionFactory(Proteus.Settings!.CustomProvider)));
        }


        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="DbConfig"/>.
        /// </summary>
        public DbConfig()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());

            if (_factories.FirstOrDefault(p => p.Predicate()) is { } f)
            {
                SetDefaultConnectionFactory(f.Factory());
            }
        }
    }
}