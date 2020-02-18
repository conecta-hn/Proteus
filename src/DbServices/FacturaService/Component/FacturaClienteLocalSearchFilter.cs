using System;
using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Component
{
    public class FacturaClienteLocalSearchFilter : IModelLocalSearchFilter
    {
        public List<TModel> Filter<TModel>(List<TModel> collection, string query) where TModel : ModelBase
        {
            return collection.Where(p => (p as FacturaBase)?.Cliente.Name.ToLower().Contains(query.ToLower()) ?? false).ToList();
        }

        public bool UsableFor(Type model)
        {
            return model.Implements<FacturaBase>();
        }
    }

    public class ClienteRtnLocalSearchFilter : IModelLocalSearchFilter
    {
        public List<TModel> Filter<TModel>(List<TModel> collection, string query) where TModel : ModelBase
        {
            return collection.Where(p => (p as Cliente)?.Rtn?.ToLower().Contains(query.ToLower()) ?? false).ToList();
        }

        public bool UsableFor(Type model)
        {
            return model.Implements<Cliente>();
        }
    }
}