using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class IsvExoneracionType : ModelBase<int>
    {
        public IsvExoApplyRule ApplyRule { get; set; }
        public virtual ICollection<FacturableCategory> RuleItems { get; set; } = new List<FacturableCategory>();

        public override string ToString()
        {
            return ApplyRule switch
            {
                IsvExoApplyRule.All => "Todos los productos",
                IsvExoApplyRule.AllBut => "A todo, excepto lo especificado",
                IsvExoApplyRule.Only => "Únicamente a lo especificado",
                _ =>"Regla desconocida",
            };
        }
    }
}