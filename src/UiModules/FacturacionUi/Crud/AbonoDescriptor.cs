using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="Abono"/>.
    /// </summary>
    public class AbonoDescriptor : CrudDescriptor<Abono>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="Abono"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            DateProperty(p => p.Timestamp).Timestamp().AsListColumn().ShowInDetails();
            NumericProperty(p => p.Amount).Positive().Label("Monto del pago").AsListColumn("C").ShowInDetails();
        }
    }
}