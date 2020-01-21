using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using TheXDS.MCART;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Types;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Conecta.Api;
using TheXDS.Proteus.Conecta.Models;
using TheXDS.Proteus.Misc;
using TheXDS.Proteus.ViewModels.Base;
using static TheXDS.MCART.Types.Extensions.EnumerableExtensions;
using static TheXDS.MCART.Types.Extensions.FlowDocumentExtensions;
using static TheXDS.MCART.Types.Extensions.StringExtensions;
using QE = System.Data.Entity.QueryableExtensions;

namespace TheXDS.Proteus.ConectaModule.ViewModels
{
    public enum ReportInto : byte
    {
        [Name("Vista previa")] Preview,
        [Name("Reporte a Excel")] Excel
    }

    public class ListReportGenViewModel : PageViewModel
    {
        private bool _withCost = true;
        private bool _withPrice = true;
        private ReportInto _into;
        private string _query = string.Empty;
        private Visibility _showGenerated;
        private FlowDocument? _generated;
        private bool _grouped;

        /// <summary>
        ///     Obtiene o establece el valor Grouped.
        /// </summary>
        /// <value>El valor de Grouped.</value>
        public bool Grouped
        {
            get => _grouped;
            set => Change(ref _grouped, value);
        }

        /// <summary>
        ///     Obtiene o establece el valor Query.
        /// </summary>
        /// <value>El valor de Query.</value>
        public string Query
        {
            get => _query;
            set => Change(ref _query, value);
        }
        
        /// <summary>
        ///     Obtiene o establece el valor WithCost.
        /// </summary>
        /// <value>El valor de WithCost.</value>
        public bool WithCost
        {
            get => _withCost;
            set => Change(ref _withCost, value);
        }

        /// <summary>
        ///     Obtiene o establece el valor WithPrice.
        /// </summary>
        /// <value>El valor de WithPrice.</value>
        public bool WithPrice
        {
            get => _withPrice;
            set => Change(ref _withPrice, value);
        }

        /// <summary>
        ///     Obtiene o establece el valor Into.
        /// </summary>
        /// <value>El valor de Into.</value>
        public ReportInto Into
        {
            get => _into;
            set => Change(ref _into, value);
        }

        /// <summary>
        /// Enumera las opciones de generación del reporte.
        /// </summary>
        public IEnumerable<NamedObject<ReportInto>> IntoOptions => NamedObject<ReportInto>.FromEnum();
        
        /// <summary>
        ///     Obtiene o establece el valor ShowGenerated.
        /// </summary>
        /// <value>El valor de ShowGenerated.</value>
        public Visibility ShowGenerated
        {
            get => _showGenerated;
            private set => Change(ref _showGenerated, value);
        }
                
        /// <summary>
        ///     Obtiene el comando relacionado a la acción Generate.
        /// </summary>
        /// <returns>El comando Generate.</returns>
        public SimpleCommand GenerateCommand { get; }

        /// <summary>
        ///     Obtiene el comando relacionado a la acción Print.
        /// </summary>
        /// <returns>El comando Print.</returns>
        public ICommand PrintCommand { get; }

        /// <summary>
        ///     Obtiene o establece el valor Generated.
        /// </summary>
        /// <value>El valor de Generated.</value>
        public FlowDocument? Generated
        {
            get => _generated;
            private set => Change(ref _generated, value);
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="ListReportGenViewModel"/>.
        /// </summary>
        /// <param name="host"></param>
        public ListReportGenViewModel(ICloseable host) : base(host)
        {
            Title = "Generar reporte de listado de productos";
            GenerateCommand = new SimpleCommand(OnGenerate);
            PrintCommand = new SimpleCommand(OnPrint);
        }

        private void OnPrint()
        {
            if (!(Generated is null)) Prnt(Generated);
        }
        private async void OnGenerate()
        {
            IsBusy = true;
            var l = await QueryUpAsync();
            switch (Into)
            {
                case ReportInto.Preview:
                    if (Grouped) await MakeGroupProducts();
                    else MakeListProductsPrint(l);
                    break;
                case ReportInto.Excel:
                    MakeListProductsExcel(l);
                    break;
                default:
                    throw new NotImplementedException();
            }
            IsBusy = false;
        }

        private void MakeListProductsExcel(List<Item> l)
        {
            var sfd = new SaveFileDialog() { Filter = "Archivo de Excel (*.xlsx)|*.xlsx" };
            if (!(sfd.ShowDialog() ?? false)) return;
            var f = new FileInfo(sfd.FileName);

            var title = "Lista de productos";
            using var doc = new ExcelPackage();
            var ws = PrepareListProducts(doc, title, out var row, out var lastCol);

            foreach (var j in l)
            {
                var col = 3;
                if (j.Parent is null) continue;

                ws.Cells[row, 1].Value = j.Parent.Name;
                ws.Cells[row, 2].Value = GetDesc(j);
                if (WithCost) ws.Cells[row, col++].Value = CalcCosto(j);
                if (WithPrice) ws.Cells[row, col].Value = CalcPrecio(j);

                ws.Row(row).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                ws.Cells[row, 2].Style.WrapText = true;
                row++;
            }

            ws.Column(1).AutoFit();            
            for (var cc = 3;cc<=lastCol;cc++) ws.Column(cc).AutoFit();
            doc.SaveAs(f);
        }
        private void MakeListProductsPrint(List<Item> l)
        {
            var fd = MkRprt("Lista de productos");
            fd.Text($"Total de artículos: {l.Count}");

            var cols = new Dictionary<string, GridLength>();
            cols.Add("Ítem", new GridLength(3, GridUnitType.Star));
            cols.Add("Descripción", new GridLength(4, GridUnitType.Star));
            if (WithCost) cols.Add("Costo", new GridLength(2, GridUnitType.Star));
            if (WithPrice) cols.Add("Precio", new GridLength(2, GridUnitType.Star));

            var tbl = fd.AddTable(cols);

            tbl.BorderBrush = Brushes.Black;
            tbl.BorderThickness = new Thickness(3);

            var flag = false;

            foreach (var j in l)
            {
                if (j.Parent is null) continue;
                var rowdata = new List<string>();
                rowdata.Add(j.Parent.Name);
                rowdata.Add(GetDesc(j));
                if (WithCost) rowdata.Add(CalcCosto(j));
                if (WithPrice) rowdata.Add(CalcPrecio(j));
                var row = AddR(tbl, rowdata);
                if (flag)
                {
                    flag = false;
                    row.Background = Brushes.LightGray;
                }
                else { flag = true; }
            }
            Generated = fd;
        }

        private async Task<List<Item>> QueryUpAsync()
        {
            return Query.IsEmpty()
                ? (await QE.ToListAsync(Proteus.Service<ConectaService>().All<Item>())).Where(NoPagado).ToList()
                : (await QE.ToListAsync(Internal.Query(Query, typeof(Lote)))).Cast<Lote>().SelectMany(p => p.Items).Where(NoPagado).ToList();
        }
        private static bool NoPagado(Item item)
        {
            if (item.MenudeoParent is { } m)
            {
                var t = m.Pagos.Any() ? m.Pagos.Sum(p=>p.Abono) : 0m;
                return t != m.Total;
            }
            return true;
        }
        private static void Prnt(FlowDocument fd, string? title = null)
        {
            try
            {
                fd.Print($"{title ?? RprtName()} - Proteus");
            }
            catch
            {
                Proteus.MessageTarget?.Stop("Hubo un problema al imprimir el reporte.");
            }
        }
        private static FlowDocument MkRprt(string? title = null)
        {
            return Reporting.ReportBuilder.MakeReport(title ?? RprtName());
        }
        private static string RprtName(int lvl = 2)
        {
            return $"{ReflectionHelpers.GetCallingMethod(lvl)?.GetAttr<NameAttribute>()?.Value ?? "Reporte"}";
        }
        private static string CalcPrecio(Item i)
        {
            return (i.Parent.UnitVenta, i.Descuento) switch
            {
                (decimal v, null) => v.ToString("C"),
                (decimal v, decimal d) => $"{v:C} ({v - d:C} lo menos)",
                _ => "Pregunte"
            };
        }
        private static string CalcCosto(Item i)
        {
            return CalcCosto(i.Parent);
        }
        private static string CalcCosto(Lote i)
        {
            if (i.Inversion.Any())
            {
                return (i.Inversion.Sum(p => p.Total) / i.Items.Count).ToString("C");
            }
            return string.Empty;
        }
        private static string GetDesc(Item i)
        {
            return string.Join(Environment.NewLine, new string?[] { i.Parent.Description, i.Description }.NotNull()).OrNull() ?? "-";
        }
        private static TableRow AddR(Table tbl, IEnumerable<string> values)
        {
            var lst = values.ToList();

            if (lst.Count > tbl.Columns.Count) throw new ArgumentOutOfRangeException();

            var rg = new TableRowGroup();
            var row = new TableRow();
            foreach (var j in lst) row.Cells.Add(new TableCell(new Paragraph(new Run(j))));
            rg.Rows.Add(row);

            tbl.RowGroups.Add(rg);

            return row;
        }
        private ExcelWorksheet PrepareListProducts(ExcelPackage xl, string title, out int firstRow, out int lastColumn)
        {
            var ws = xl.Workbook.Worksheets.Add(title);

            void SetColsCurrency(params int[] indexes)
            {
                foreach (var j in indexes)
                {
                    ws.Column(j).Width = 26.43;
                    ws.Column(j).StyleName = "Currency";
                }
            }
            void SetRowsFormat(int sze, params int[] indexes)
            {
                foreach (var j in indexes)
                {
                    ws.Cells[j, 1, j, sze].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells[j, 1, j, sze].Style.Fill.BackgroundColor.SetColor(new TheXDS.MCART.Types.Color(0x3c, 0xc2, 0xe7));
                    ws.Cells[j, 1, j, sze].Style.Font.Color.SetColor(TheXDS.MCART.Resources.Colors.White);
                }
            }

            ws.Cells[1, 1].Value = Proteus.Settings!.BusinessName;
            ws.Cells[2, 1].Value = title;
            ws.Cells[2, 1].Style.Font.Size *= 1.5f;
            SetRowsFormat(3, 1, 2, 3);
            SetColsCurrency(3);
            ws.Cells[3, 1].Value = "Ítem";
            ws.Cells[3, 2].Value = "Descripción";
            lastColumn = 2;

            if (WithCost)
            {
                ws.Cells[3, ++lastColumn].Value = "Costo";
                SetColsCurrency(lastColumn);
            }

            if (WithPrice)
            {
                ws.Cells[3, ++lastColumn].Value = "Precio";
                SetColsCurrency(lastColumn);
            }

            ws.Column(2).Width = 43;
            firstRow = 4;
            return ws;
        }
        private async Task MakeGroupProducts()
        {
            var fd = Reporting.ReportBuilder.MakeReport("Lista agrupada de productos");
            List<Lote> l;
            if (Query.IsEmpty())
            {
                l = await QE.ToListAsync(Proteus.Service<ConectaService>().All<Lote>());
            }
            else
            {
                l = (await QE.ToListAsync(Internal.Query(Query, typeof(Lote)))).Cast<Lote>().ToList();
            }

            fd.Text($"Total de productos distintos: {l.Count}");
            fd.Text($"Gran total de artículos: {l.SelectMany(p => p.Items).Where(p => p.MenudeoParent is null).Count()}");


            foreach (var j in l.NotNull())
            {
                var itms = j.Items.Where(NoPagado).ToList();
                if (itms.Count == 0) continue;
                fd.Title(j.Name, 3);
                fd.Text(string.Join(Environment.NewLine, new[] {
                                j.Description.OrNull() ?? "",
                                WithCost ? $"Costo: {CalcCosto(j)}" : null,
                                WithPrice ? $"Precio de venta: {j.UnitVenta?.ToString("C") ?? "Pregunte"}" : null,
                                $"Total de artículos: {itms.Count}"
                            }.NotNull()));
                var ll = itms.Where(p => (!p.Description.IsEmpty()) || p.Descuento.HasValue).ToList();
                if (ll.Any())
                {
                    fd.Text($"Ítems con detalles: {ll.Count}");

                    var cols = new Dictionary<string, GridLength>();
                    cols.Add("Detalle", new GridLength(2, GridUnitType.Star));
                    if (WithCost) cols.Add("Costo", new GridLength(1, GridUnitType.Star));
                    if (WithPrice) cols.Add("Precio", new GridLength(1, GridUnitType.Star));
                    var tbl = fd.AddTable(cols.ToArray());
                    tbl.BorderBrush = Brushes.Black;
                    tbl.BorderThickness = new Thickness(3);
                    var flag = false;
                    foreach (var k in ll)
                    {
                        var row = AddR(tbl, new string?[]
                        {
                            k.Description ?? "-",
                            WithCost ? CalcCosto(k): null,
                            WithPrice ? CalcPrecio(k): null
                        }.NotNull());
                        if (flag)
                        {
                            flag = false;
                            row.Background = Brushes.LightGray;
                        }
                        else { flag = true; }
                    }
                }
                fd.Blocks.Add(new BlockUIContainer(new Separator()));
            }
            Generated = fd;            
        }

    }
}
