using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Component
{
    public class FacturaClienteLocalSearchFilter : ModelLocalSearchFilter<Factura>
    {
        public override bool Filter(Factura element, string query)
        {
            return element.Cliente.Name.ToLower().Contains(query.ToLower());
        }
    }
}