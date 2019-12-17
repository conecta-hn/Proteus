/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    ///     Genera conexiones de base de datos hacia un servidor de SQL Server
    ///     dentro del dominio.
    /// </summary>
    public class DomainProviderFactory : IDbConnectionFactory
    {
        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="DomainProviderFactory"/>.
        /// </summary>
        /// <param name="server">
        ///     Servidor de SQL Server al cual conectarse dentro del dominio.
        /// </param>
        public DomainProviderFactory(string server)
        {
            if (string.IsNullOrWhiteSpace(server)) throw new ArgumentException();
            Server = server;
        }
        /// <summary>
        ///     Servidor de SQL Server al cual conectarse dentro del dominio.
        /// </summary>
        public string Server { get; }
        /// <summary>
        ///     Crea una nueva conexión de datos hacia el servidor de SQL
        ///     Server especificado dentro del dominio.
        /// </summary>
        /// <param name="nameOrConnectionString">
        ///     Nombre o cadena de conexión a pasar al crear la conexión.
        /// </param>
        /// <returns>
        ///     Una conexión de base de datos hacia el servidor de SQL Server
        ///     especificado dentro del dominio.
        /// </returns>
        public DbConnection CreateConnection(string nameOrConnectionString)
        {
            return new SqlConnectionFactory($"Data Source={Server};Integrated Security=True; MultipleActiveResultSets=True;").CreateConnection(nameOrConnectionString);
        }
    }
}