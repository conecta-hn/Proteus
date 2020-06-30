/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System.Linq;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Component
{
    public abstract class ModelLocalSearchFilter<T> : ModelSearchFilterBase<T>, IModelLocalSearchFilter where T : ModelBase
    {
        public List<TModel> Filter<TModel>(List<TModel> collection, string query) where TModel : ModelBase
        {
            return collection.Where(p => p is T q && Filter(q, query)).ToList();
        }

        public abstract bool Filter(T element, string query);
    }
}