using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="FacturaXCobrar"/>.
    /// </summary>
    public class FacturaXCobrarDescriptor : CrudDescriptor<FacturaXCobrar>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="FacturaXCobrar"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            FriendlyName("Cuenta por cobrar");
            DateProperty(p => p.Timestamp).Timestamp().AsListColumn().ShowInDetails();
            ObjectProperty(p => p.Cliente).Selectable().Required().AsListColumn().ShowInDetails();
            ObjectProperty(p => p.Parent).Selectable().Label("Factura a pagar").Required().AsListColumn().ShowInDetails();
            NumericProperty(p => p.Total).Positive().AsListColumn().ShowInDetails();
            ListProperty(p => p.Abonos).Creatable();
            Property(p => p.IsPaid)
                .Label("Cuenta pagada")
                .ShowInDetails()
                .AsListColumn()
                .Hidden();
            BeforeSave(SetIsPaid);
        }

        private void SetIsPaid(FacturaXCobrar arg1, ModelBase arg2)
        {
            arg1.IsPaid = arg1.Pending == 0m;
        }
    }
}