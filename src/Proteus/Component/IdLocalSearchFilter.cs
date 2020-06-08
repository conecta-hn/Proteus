/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System;
using TheXDS.Proteus.Models.Base;
using System.Linq;

namespace TheXDS.Proteus.Component
{
    public class IdLocalSearchFilter : IModelLocalSearchFilter
    {
        public List<TModel> Filter<TModel>(List<TModel> collection, string query) where TModel : ModelBase
        {
            return collection.Where(p => p.StringId.Contains(query)).ToList();
        }

        public bool UsableFor(Type model) => true;
    }
}