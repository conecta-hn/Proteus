/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Api
{

    public interface IWriteService
    {
        bool Add<TEntity>(TEntity newEntity) where TEntity : ModelBase;
        bool Add<TEntity>(IEnumerable<TEntity> newEntities) where TEntity : ModelBase;
    }
}