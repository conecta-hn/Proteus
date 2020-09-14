/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component.Attributes;
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
        public InvCategoryCrudTool() : base(CrudToolVisibility.NotEditing)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public override IEnumerable<Launcher> GetLaunchers(IEnumerable<Type> models, ICrudViewModel? vm)
        {
            yield return Launcher.FromMethod(
                "Exportar a Excel",
                "Exporta la lista de ítems dentro de esta categoría a un archivo de Microsoft Excel.",
                ExportExcel, () => vm!);
        }

        private void ExportExcel(ICrudViewModel? vm)
        {
            if (vm is { } v) v.BusyDo(ExportExcelAsync(vm));
            else ExportExcelAsync(null).GetAwaiter().GetResult();
        }
        private async Task ExportExcelAsync(ICrudViewModel? vm)
        {
            var sfd = new SaveFileDialog()
            {
                Filter = "Archivo de Excel (*.xlsx)|*.xlsx|Todos los archivos|*.*"
            };
            if (!(sfd.ShowDialog() ?? false)) return;
            var e = new ExcelPackage();
            foreach (var i in vm?.Selection is FacturableCategory m ? new[] { m }.AsEnumerable() : await GetCategoriesAsync())
            {
                AppendInvCategory(e, i);
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

        protected virtual IEnumerable<FacturableCategory> GetCategories()
        {
            return GetCategoriesAsync().GetAwaiter().GetResult();
        }

        protected Task<List<FacturableCategory>> GetCategoriesAsync()
        {
            return Proteus.Service<FacturaService>()!.All<FacturableCategory>().ToListAsync();
        }

        protected void AppendInvCategory(ExcelPackage e, FacturableCategory i)
        {
            var c = CrudElement.GetDescription(typeof(Producto))!.ListColumns
                .Where(p => (p as Column)?.Path != "Category")
                .ToList();

            c.Add(new CustomColum("Existencias", "0", GetExistencias));
            c.Add(new CustomColum("Descripción", GetDescr));

            var lst = i.Children.OfType<Producto>().ToArray();
            if (!lst.Any()) return;
            var ws = BuildWs(e, i, c, out var row, out var lastCol);
            foreach (var j in lst)
            {
                int col = 1;
                foreach (var l in c)
                {
                    var v = l.GetValue(j);
                    switch (v)
                    {
                        case decimal currency:
                            ws.Cells[row, col].Value = currency;
                            ws.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells[row, col].Style.Numberformat.Format = l.Format == "C" ? @"_-L* #,##0.00_-;-L* #,##0.00_-;_-L* "" - ""??_-;_-@_-" : l.Format;
                            break;
                        case int integer:
                            ws.Cells[row, col].Value = integer;
                            ws.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells[row, col].Style.Numberformat.Format = l.Format.IsEmpty() ? "0" : l.Format;
                            break;
                        case float single:
                            ws.Cells[row, col].Value = single;
                            ws.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells[row, col].Style.Numberformat.Format = l.Format.IsEmpty() ? "0.0" : l.Format;
                            break;
                        case double dbl:
                            ws.Cells[row, col].Value = dbl;
                            ws.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells[row, col].Style.Numberformat.Format = l.Format.IsEmpty() ? "0.0" : l.Format;
                            break;
                        case string str:
                            ws.Cells[row, col].Value = str;
                            ws.Cells[row, col].Style.HorizontalAlignment = str.Contains('\n') ? ExcelHorizontalAlignment.Left : ExcelHorizontalAlignment.Center;
                            ws.Cells[row, col].Style.VerticalAlignment = str.Contains('\n') ? ExcelVerticalAlignment.Top : ExcelVerticalAlignment.Center;
                            break;
                        case null : break;
                        default:
                            ws.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            ws.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            ws.Cells[row, col].Value = l.ToString(j);
                            break;
                    }
                    col++;
                }
                row++;
            }
            
            ws.Column(lastCol).Width = 45;
            ws.Column(lastCol).Style.WrapText = true;

            for (int k = 1; k <= lastCol-1; k++)
            {
                ws.Column(k).AutoFit();
            }
        }

        private object? GetExistencias(ModelBase arg2)
        {
            var p = arg2 as Producto ?? throw new InvalidTypeException(arg2.GetType());
            return p.Batches.Sum(p => p.Qty);
        }

        private object? GetDescr(ModelBase arg2)
        {
            var p = arg2 as Producto ?? throw new InvalidTypeException(arg2.GetType());
            return p.Description;
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
