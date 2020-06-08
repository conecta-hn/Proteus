using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Component
{
    public class ClienteRtnLocalSearchFilter : ModelLocalSearchFilter<Cliente>
    {
        public override bool Filter(Cliente element, string query)
        {
            return element.Rtn?.ToLower().Contains(query.ToLower()) ?? false;
        }
    }
}