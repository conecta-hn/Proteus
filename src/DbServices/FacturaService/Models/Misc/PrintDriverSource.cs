using System;
using System.Collections.Generic;
using System.Linq;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models.Misc
{
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