/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Api
{
    public interface IReadServiceBase
    {
        bool Hosts(Type tEntity);
        bool Hosts<TEntity>() where TEntity : ModelBase, new();
    }
}