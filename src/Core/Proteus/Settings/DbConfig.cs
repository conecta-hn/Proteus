/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;

namespace TheXDS.Proteus.Component
{
    /// <inheritdoc />
    /// <summary>
    /// Establece la configuración activa de servidor de base de datos.
    /// </summary>
    public class DbConfig : DbConfiguration
    {
        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="DbConfig"/>.
        /// </summary>
        public DbConfig()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
            if (Proteus.Settings.UseDomainProvider)
            {
                SetDefaultConnectionFactory(new DomainProviderFactory(Proteus.Settings.DomainProvider));
            }
            else if (Proteus.Settings.UseLocalDbProvider)
            {
                SetDefaultConnectionFactory(new LocalDbConnectionFactory("mssqllocaldb"));

            }
            else if (Proteus.Settings.UseCustomProvider)
            {
                SetDefaultConnectionFactory(new SqlConnectionFactory(Proteus.Settings.CustomProvider));
            }
        }
    }
}