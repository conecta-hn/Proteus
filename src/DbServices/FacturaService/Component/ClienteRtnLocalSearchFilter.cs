using System.Collections.Generic;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Reporting;

namespace TheXDS.Proteus.Component
{
    public class ClienteRtnLocalSearchFilter : ModelLocalSearchFilter<Cliente>
    {
        public override bool Filter(Cliente element, string query)
        {
            return element.Rtn?.ToLower().Contains(query.ToLower()) ?? false;
        }
    }

    public class ProductoDescriptionSearchFilter : ModelLocalSearchFilter<Producto>
    {
        public override bool Filter(Producto element, string query)
        {
            return element.Description?.ToLower().Contains(query.ToLower()) ?? false; ;
        }
    }
}