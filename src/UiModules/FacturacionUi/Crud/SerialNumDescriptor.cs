using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    public class SerialNumDescriptor : CrudDescriptor<SerialNum>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Ítem con número de serie");
            Property(p => p.Id).Id("Número de serie", "|||").AsListColumn();
            TextProperty(p => p.Notes).Big().Nullable().Label("Notas");
            ObjectProperty(p => p.Owner).Selectable().Nullable().Label("Vendido a");
            Property(p => p.Sold).Nullable().Label("Vendido el");
            ShowAllInDetails();
        }
    }
}