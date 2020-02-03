using System;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.FacturacionUi.ViewModels;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="Paquete"/>.
    /// </summary>
    public class PaqueteDescriptor : CrudDescriptor<Paquete, PaqueteCrudViewModel>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="Paquete"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            this.DescribeFacturable();
            ListProperty(p => p.Children)
                .Selectable()
                .Label("Ítems contenidos en la promoción");
            DateProperty(p => p.ValidFrom)
                .Required()
                .Label("Válida desde")
                .Default(DateTime.Now);
            DateProperty(p => p.Void)
                .Nullable()
                .Label("Fecha de vencimiento")
                .AsListColumn()
                .ShowInDetails()
                .Default(DateTime.Now.AddMonths(1));

            NumericProperty(p => p.Precio).Hidden();

            VmProperty(p => p.Price)
                .Important("Precio de venta").RadioSelectable();
            VmProperty(p => p.Percent)
                .Important("Porcentaje de descuento").RadioSelectable();

        }
    }
}