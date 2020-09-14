/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Plugins;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.FacturacionUi.Tools
{
    public class InvCategoryCrudTool : CrudTool<FacturableCategory>
    {
        public InvCategoryCrudTool() : base(CrudToolVisibility.Unselected)
        {
        }

        public override IEnumerable<Launcher> GetLaunchers(IEnumerable<Type> models, ICrudViewModel? vm)
        {
            yield return Launcher.FromMethod(
                "Exportar a Excel",
                "Exporta la lista de ítems dentro de esta categoría a un archivo de Microsoft Excel.",
                ExportExcel, () => vm);
        }


        private async void ExportExcel(ICrudViewModel? obj)
        {
            var sfd = new SaveFileDialog()
            {
                Filter = "Archivo de Excel (*.xlsx)|*.xlsx|Todos los archivos|*.*"
            };
            if (!(sfd.ShowDialog() ?? false)) return;

            var e = new ExcelPackage();
            var c = CrudElement.GetDescription(typeof(Producto))!.ListColumns.ToList();
            c.Add(new CustomColum("Existencias", "C", GetExistencias));

            foreach (var i in await Proteus.Service<FacturaService>()!.All<FacturableCategory>().ToListAsync())
            {
                var lst = i.Children.OfType<Producto>().ToArray();
                if (!lst.Any()) continue;
                var ws = BuildWs(e, i, c, out var row, out var lastCol);
                foreach (var j in lst)
                {
                    int col = 1;
                    foreach (var l in c)
                    {
                        ws.Cells[row, col].Value = l.ToString(i);
                        col++;
                    }
                    row++;
                }
                for (int k = 1; k <= lastCol; k++)
                {
                    ws.Column(k).AutoFit();
                }
            }

            if (!e.Workbook.Worksheets.Any())
            {
                Proteus.MessageTarget?.Warning("Nada que presentar. No hay productos definidos ni en inventario.");
            }
            else
            {
                e.SaveAs(new FileInfo(sfd.FileName));
            }
        }

        private object? GetExistencias(ModelBase arg2)
        {
            var p = arg2 as Producto ?? throw new InvalidTypeException(arg2.GetType());
            return p.Batches.Sum(p => p.Qty);
        }

        private ExcelWorksheet BuildWs(ExcelPackage e, FacturableCategory f, IEnumerable<IColumn> c, out int row, out int lastCol)
        {
            var ws = e.Workbook.Worksheets.Add(f.Name);
            ws.Cells[1, 1].Value = Proteus.Settings?.BusinessName ?? string.Empty;
            ws.Cells[2, 1].Value = "Listado de inventario para " + f.Name;
            ws.Cells[2, 1].Style.Font.Size *= 1.3f;
            ws.Cells[3, 1].Value = string.Format("Reporte generado el {0}", DateTime.Now);
            lastCol = 0;
            foreach (var i in c)
            {
                lastCol++;
                ws.Cells[4, lastCol].Value = i.Header;
            }

            bool flag = lastCol > 0;
            if (flag)
            {
                ws.Cells[4, 1, 4, lastCol].Style.Fill.PatternType = ExcelFillStyle.Solid;
                var backgroundColor = ws.Cells[4, 1, 4, lastCol].Style.Fill.BackgroundColor;
                var lightSlateGray = TheXDS.MCART.Resources.Colors.LightSlateGray;
                backgroundColor.SetColor(lightSlateGray);
            }
            for (int j = 1; j < 4; j++)
            {
                ws.Cells[j, 1, j, lastCol].Merge = true;
            }
            row = 5;
            return ws;
        }
    }
}
