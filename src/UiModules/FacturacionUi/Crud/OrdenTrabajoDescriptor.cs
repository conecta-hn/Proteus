using System;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.FacturacionUi.Modules;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Pages;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="OrdenTrabajo"/>.
    /// </summary>
    public class OrdenTrabajoDescriptor : CrudDescriptor<OrdenTrabajo>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="OrdenTrabajo"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool | InteractionType.Essential);
            FriendlyName("Orden de trabajo");
            Property(p => p.Id).ReadOnly();
            DateProperty(p => p.Timestamp)
                .WithTime()
                .Default(DateTime.Now)
                .Label("Fecha de creación")
                .AsListColumn()
                .ShowInDetails()
                .ReadOnly();
            DateProperty(p => p.Entrega)
                .Default(DateTime.Now + TimeSpan.FromDays(2))
                .Label("Fecha de entrega")
                .AsListColumn()
                .ShowInDetails()
                .ReadOnly();
            ObjectProperty(p => p.Cliente)
                .Selectable().Creatable()
                .Important()
                .AsListColumn()
                .ShowInDetails()
                .Required();
            ListProperty(p => p.Items).Creatable().ShowInDetails();
            TextProperty(p => p.Notas).Big().ShowInDetails();
            Property(p => p.Facturado).ShowInDetails().AsListColumn().ReadOnly();
            AfterSave(PrintOrden);
        }

        private void PrintOrden(OrdenTrabajo arg1, ModelBase arg2)
        {
            FacturaService.PrintOt(arg1);
        }
    }
}