using System.Linq;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Misc;

namespace TheXDS.Proteus.FacturacionUi.ViewModels
{
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
}