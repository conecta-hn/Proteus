/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using TheXDS.Proteus.Component;

namespace TheXDS.Proteus.Context
{
    [DbConfigurationType(typeof(DbConfig))]
    public abstract class ProteusContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            modelBuilder.Conventions.Add(new DecimalPropertyConvention(38, 18));
        }
    }
}