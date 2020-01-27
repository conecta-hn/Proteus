/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Data.Entity;
using TheXDS.Proteus.Component;

namespace TheXDS.Proteus.Context
{
    [DbConfigurationType(typeof(DbConfig))]
    public abstract class ProteusContext : DbContext { }
}