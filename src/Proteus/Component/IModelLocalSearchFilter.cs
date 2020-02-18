/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System;
using TheXDS.Proteus.Models.Base;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using System.Linq;

namespace TheXDS.Proteus.Component
{
    public interface IModelLocalSearchFilter
    {
        List<TModel> Filter<TModel>(List<TModel> collection, string query) where TModel : ModelBase;

        /// <summary>
        /// Comprueba si este <see cref="IModelSearchFilter"/> puede crear
        /// filtros para el modelo especificado.
        /// </summary>
        /// <param name="model">Modelo a comprobar.</param>
        /// <returns>
        /// <see langword="true"/> si esta instancia puede crear filtros de
        /// consulta para el modelo especificado, <see langword="false"/>
        /// en caso contrario.
        /// </returns>
        bool UsableFor(Type model);
    }

    public class NameLocalSearchFilter : IModelLocalSearchFilter
    {
        public List<TModel> Filter<TModel>(List<TModel> collection, string query) where TModel : ModelBase
        {
            return collection.Where(p => ((INameable)p).Name.ToLower().Contains(query.ToLower())).ToList();
        }

        public bool UsableFor(Type model)
        {
            return model.Implements<INameable>();
        }
    }

    public class IdLocalSearchFilter : IModelLocalSearchFilter
    {
        public List<TModel> Filter<TModel>(List<TModel> collection, string query) where TModel : ModelBase
        {
            return collection.Where(p => p.StringId.Contains(query)).ToList();
        }

        public bool UsableFor(Type model)
        {
            return true;
        }
    }
}