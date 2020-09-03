using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    public class LoteDescriptor : CrudDescriptor<Lote>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool); 
            FriendlyName("Lote");

            Property(p => p.Id).Id("Código de lote")
                .ShowInDetails()
                .AsListColumn();

            DateProperty(p => p.Manufactured)
                .Label("Fecha de manufactura")
                .Required()
                .ShowInDetails()
                .AsListColumn();

            ListProperty(p => p.OnBatches)
                .Label("Bloques relacionados")
                .Required().ShowInDetails();
        }
    }
}