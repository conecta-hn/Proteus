using System;
using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models.Misc
{
    public class PrinterSource : ModelBase<int>
    {
        public static IQueryable<PrinterSource> GetPrinters()
        {
            try
            {
                var l = new List<PrinterSource>();
                foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters.Cast<string?>().NotNull())
                {
                    l.Add(new PrinterSource() { Printer = printer });
                }
                return l.AsQueryable();
            }
            catch (Exception ex)
            {
                if (Proteus.CommonReporter?.IsBusy ?? false)
                {
                    Proteus.CommonReporter.UpdateStatus(ex.Message);
                }
                else if (Proteus.AlertTarget is { } a)
                {
                    a.Alert("Error al obtener las impresoras del sistema", ex.Message);
                }
                else
                {
                    Proteus.MessageTarget?.Error(ex.Message);
                }
                return Array.Empty<PrinterSource>().AsQueryable();
            }
        }

        public static IQueryable<PrinterSource> Printers { get; } = GetPrinters();


        public string? Printer { get; set; }

        public override string ToString() => Printer ?? string.Empty;
    }
}