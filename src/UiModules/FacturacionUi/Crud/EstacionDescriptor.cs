using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.FacturacionUi.ViewModels;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Models.Misc;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="Estacion"/>.
    /// </summary>
    public class EstacionDescriptor : CrudDescriptor<Estacion, EstacionViewModel>
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
            NumericProperty(p => p.SecondScreen).Range(2, 255).Nullable().Label("Pantalla secundaria")
                .Tooltip("Permite habilitar el uso de una pantalla secundaria visibie al cliente.");
            NumericProperty(p => p.MinFacturasAlert).Range(0, 99999999).Nullable().Label("Nivel de alerta de mínimo de facturas");
            ObjectProperty(p => p.Bodega)
                .Selectable()
                .Creatable()
                .Nullable()
                .Label("Bodega de salida de inventario").ShowInDetails()
                .Tooltip("Permite establecer la bodega de salida de los productos facturados en esta estación. Si no se establece una bodega de salida, únicamente se podrán facturar servicios en esta estación.");
            Property(p => p.Printer).Label("Nombre de impresora").OnlyInDetails();

            VmObjectProperty(p => p.SelectedPrinter).Selectable().Source(PrinterSource.Printers).Required().Label("Impresora");
            VmObjectProperty(p => p.SelectedPrintDriver).Selectable().Source(PrintDriverSource.Drivers).Required().Label("Controlador de impresora");

            VmBeforeSave(SetPrinter).Then(SetDriver);
        }

        private void SetPrinter(EstacionViewModel arg1, ModelBase arg2)
        {
            if (arg1.SelectedPrinter?.Printer is { } p)
                arg1.Entity.Printer = p;
        }

        private void SetDriver(EstacionViewModel arg1, ModelBase arg2)
        {
            if (arg1.SelectedPrintDriver?.DriverGuid is { } p)
                arg1.Entity.PrintDriver = p;
        }
    }
}