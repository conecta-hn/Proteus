using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="Estacion"/>.
    /// </summary>
    public class EstacionDescriptor : CrudDescriptor<Estacion>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="Estacion"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.Settings);
            FriendlyName("Estación de facturación");

            this.DescribeEstacion();
            ObjectProperty(p => p.Entidad).Selectable().Required().Important("Entidad de facturación");
            ListProperty(p => p.RangosAsignados)
                .Selectable()
                .Source(FacturaService.UnassignedRangos)
                .Label("Rangos de facturación asignados");
            NumericProperty(p => p.SecondScreen).Range(2, 255).Nullable().Label("Pantalla secundaria");
            NumericProperty(p => p.MinFacturasAlert).Range(0, 99999999).Nullable().Label("Nivel de alerta de mínimo de facturas");
        }
    }
}