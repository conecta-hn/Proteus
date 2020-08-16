using System;
using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;

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
            NumericProperty(p => p.SecondScreen).Range(2, 255).Nullable().Label("Pantalla secundaria");
            NumericProperty(p => p.MinFacturasAlert).Range(0, 99999999).Nullable().Label("Nivel de alerta de mínimo de facturas");
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

    /// <summary>
    /// Clase base personalizada para el ViewModel recompilado que se utilizará
    /// dentro del Crud generado para el modelo
    /// <see cref="Estacion"/>.
    /// </summary>
    public class EstacionViewModel : ViewModel<Estacion>
    {
        private PrinterSource? _selectedPrinter = null!;
        private PrintDriverSource? _selectedPrintDriver = null!;

        /// <summary>
        ///     Obtiene o establece el valor SelectedPrinter.
        /// </summary>
        /// <value>El valor de SelectedPrinter.</value>
        public PrinterSource SelectedPrinter
        {
            get => _selectedPrinter ?? PrinterSource.Printers.FirstOrDefault(p=>p.Printer == Entity.Printer);
            set => Change(ref _selectedPrinter, value);
        }

        /// <summary>
        ///     Obtiene o establece el valor SelectedPrintDriver.
        /// </summary>
        /// <value>El valor de SelectedPrintDriver.</value>
        public PrintDriverSource SelectedPrintDriver
        {
            get => _selectedPrintDriver ?? PrintDriverSource.Drivers.FirstOrDefault(p => p.DriverGuid == Entity.PrintDriver);
            set => Change(ref _selectedPrintDriver, value);
        }
    }

    public class PrinterSource : ModelBase<int>
    {
        public static IQueryable<PrinterSource> GetPrinters()
        {
            var l = new List<PrinterSource>();
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string?>().NotNull())
            {
                l.Add(new PrinterSource() { Printer = printer });
            }
            return l.AsQueryable();
        }

        public static IQueryable<PrinterSource> Printers { get; } = GetPrinters();


        public string? Printer { get; set; }

        public override string ToString() => Printer ?? string.Empty;
    }

    public class PrintDriverSource : ModelBase<int>
    {
        public static IQueryable<PrintDriverSource> GetDrivers()
        {
            var l = new List<PrintDriverSource>();
            foreach (var j in FacturaService.FactPrintDrivers)
            {
                l.Add(new PrintDriverSource() { DriverGuid = j.Guid });
            }
            return l.AsQueryable();
        }

        public static IQueryable<PrintDriverSource> Drivers { get; } = GetDrivers();

        public Guid DriverGuid { get; set; }
        public override string ToString()
        {
            return FacturaService.FactPrintDrivers.FirstOrDefault(p => p.Guid == DriverGuid)?.Name ?? "n/a";
        }
    }
}