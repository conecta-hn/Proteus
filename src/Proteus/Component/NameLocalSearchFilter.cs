/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;
using TheXDS.MCART.Types.Base;
using System.Linq;

namespace TheXDS.Proteus.Component
{
    public class NameLocalSearchFilter : ModelSearchFilterBase<INameable>, IModelLocalSearchFilter
    {
        public List<TModel> Filter<TModel>(List<TModel> collection, string query) where TModel : ModelBase
        {
            return collection.Where(p => ((INameable)p).Name.ToLower().Contains(query.ToLower())).ToList();
        }
    }
}