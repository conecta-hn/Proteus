using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Conecta.Api;
using TheXDS.Proteus.Conecta.Models;
using TheXDS.Proteus.Conecta.ViewModels;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Dialogs;
using TheXDS.Proteus.Misc;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Plugins;
using static TheXDS.MCART.Types.Extensions.EnumerableExtensions;
using static TheXDS.MCART.Types.Extensions.FlowDocumentExtensions;
using static TheXDS.MCART.Types.Extensions.StringExtensions;
using static TheXDS.Proteus.Annotations.InteractionType;
using QE = System.Data.Entity.QueryableExtensions;

namespace TheXDS.Proteus.Conecta
{
    namespace Tools
    {
        //public class ExportTool : Tool
        //{
        //    [InteractionItem, Name("Importar datos")]
        //    public async void ExportAsync(object? sender, EventArgs e)
        //    {
        //        var fd = new OpenFileDialog();
        //        if (!(fd.ShowDialog() ?? false)) return;
        //        var svc = Proteus.Service<ConectaService>();

        //        using var fs = new FileStream(fd.FileName, FileMode.Open);
        //        using var bw = new BinaryReader(fs);

        //        var c = bw.ReadInt32();
        //        for(var j = 0; j < c; j++)
        //        {
        //            _ = bw.ReadInt32();
        //            _ = bw.ReadString();
        //        }

        //        c = bw.ReadInt32();
        //        for (var j = 0; j < c; j++)
        //        {
        //            _ = bw.ReadInt32();
        //            _ = bw.ReadString();
        //        }
        //        c = bw.ReadInt32();

        //        for (var j = 0; j < c; j++)
        //        {
        //            _ = bw.ReadInt64();

        //            var l = new Lote
        //            {
        //                Name = bw.ReadString()
        //            };
        //            var ns = bw.ReadString();
        //            if (!ns.IsEmpty())
        //            {
        //                l.Items.Add(new Item { Name = ns });
        //            }
        //            _ = bw.ReadInt32();
        //            l.Description = bw.ReadString();
        //            l.Timestamp = DateTime.FromBinary(bw.ReadInt64());
        //            var cc = bw.ReadInt32();
        //            while (l.Items.Count < cc)
        //            {
        //                l.Items.Add(new Item());
        //            }
        //            _ = bw.ReadDecimal();
        //            _ = bw.ReadDecimal();

        //            await svc.AddAsync(l);
        //        }
        //    }
        //}
    }

    namespace Crud
    {
        public class ItemDescriptor : CrudDescriptor<Item>
        {
            protected override void DescribeModel()
            {
                OnModuleMenu(Essential | Catalog);
                FriendlyName("Artículo");                
                Property(p => p.Info).Label("Descripción").AsListColumn().ShowInDetails().ReadOnly();
                Property(p => p.Name).Nullable().Important("Número de serie");
                TextProperty(p => p.Description).Big().Nullable().Important("Detalles");
                NumericProperty(p => p.Descuento).Nullable().Important("Descuento");
                ObjectProperty(p => p.MenudeoParent).Creatable().Important("Vendido a").Nullable();
            }
        }

        public class LoteDescriptor : CrudDescriptor<Lote, LoteViewModel>
        {
            protected override void DescribeModel()
            {
                OnModuleMenu(Essential | Catalog);
                FriendlyName("Lote de artículos");

                Property(p => p.Name).AsName().NotEmpty();
                ObjectProperty(p => p.Proveedor).Selectable().Nullable();
                ListProperty(p => p.Inversion).Creatable().Label("Inversiones");
                TextProperty(p => p.Description).Big().Label("Detalles");
                VmProperty(p => p.ExistenciasCount).Label("Existencias").OnlyInDetails();
                VmProperty(p => p.VendidosCount).Label("Créditos abiertos").OnlyInDetails();
                VmProperty(p => p.VendidosCount).Label("Artículos vendidos").OnlyInDetails();
                ListProperty(p => p.Items).Creatable().Label("Ítems");
                ListProperty(p => p.Pictures).Creatable().Label("Fotografías");
                DateProperty(p => p.Timestamp).WithTime().Label("Fecha/hora de ingreso").Default(DateTime.Now);
                NumericProperty(p => p.UnitVenta).Nullable().Important("Precio de venta unitario");

                ShowAllInDetails();

                CustomAction("Agregar bulk de items", OnAddItems);
                CustomAction("Agregar cantidad de items", OnAddQty);
            }

            private async void OnAddQty(Lote obj)
            {
                if (InputSplash.GetNew("Cantidad de items a agregar", out int qty))
                {
                    for (var j = 0; j< qty; j++)
                    {
                        obj.Items.Add(new Item());
                    }
                }
            }
            private async void OnAddItems(Lote obj)
            {
                var c = 0;
                while (InputSplash.GetNew($"Introduzca el nuevo número de serie. ({c++} hasta ahora)", out string sn))
                {
                    obj.Items.Add(new Item
                    {
                        Name = sn
                    });
                }
            }
        }

        public class ItemPictureDescriptor : CrudDescriptor<ItemPicture>
        {
            protected override void DescribeModel()
            {
                FriendlyName("Imagen de artículo");
                TextProperty(p => p.Path).TextKind(TextKind.PicturePath).Label("Imagen").AsListColumn();
                TextProperty(p => p.Notes).Big().Label("Notas");
                ShowAllInDetails();
            }
        }

        public class InversionDescriptor  : CrudDescriptor<Inversion, PagoViewModel<Inversion>> 
        {
            protected override void DescribeModel()
            {
                FriendlyName("Inversión");
                DateProperty(p => p.Timestamp).WithTime().Label("Fecha/hora de ingreso").Default(DateTime.Now);
                ObjectProperty(p => p.Inversor).Selectable().Required().Important();
                NumericProperty(p => p.Total).Label("Inversión total");
                ListProperty(p => p.Pagos).Creatable().Important("Abonos realizados");

                VmProperty(p => p.Pendiente).ShowInDetails().AsListColumn().ReadOnly();
                VmProperty(p => p.Abonado).ShowInDetails().AsListColumn().ReadOnly();
                VmProperty(p => p.LastPagoWhen).ShowInDetails().AsListColumn().Label("Último pago").ReadOnly();
                VmProperty(p => p.LastPagoHowMuch).ShowInDetails().AsListColumn().Label("Último pago").ReadOnly();
            }
        }

        public class Pagodescriptor : CrudDescriptor<Pago>
        {
            protected override void DescribeModel()
            {
                DateProperty(p => p.Timestamp).WithTime().Label("Fecha/hora de abono").Default(DateTime.Now);
                NumericProperty(p => p.Abono).Range(1m, decimal.MaxValue).Default(500m);
                ShowAllInDetails();
            }
        }

        public class ProveedorDescriptor : CrudDescriptor<Proveedor>
        {
            protected override void DescribeModel()
            {
                OnModuleMenu(Essential | AdminTool);

                Property(p => p.Name).AsName("Nombre del proveedor");
                this.DescribeContact();
                ListProperty(p => p.Inventario).Creatable().Label("Productos ofrecidos");

                ShowAllInDetails();
            }
        }

        public class InversorDescriptor : CrudDescriptor<Inversor>
        {
            protected override void DescribeModel()
            {
                OnModuleMenu(Essential | AdminTool);

                Property(p => p.Name).AsName("Nombre del inversor");
                this.DescribeContact();
                ListProperty(p => p.Inversion).Creatable().Label("Inversiones");

                ShowAllInDetails();
            }
        }

        public class VendedorDescriptor : CrudDescriptor<Vendedor>
        {
            protected override void DescribeModel()
            {
                OnModuleMenu(Essential | AdminTool);

                FriendlyName("Cliente");

                Property(p => p.Name).AsName("Nombre del cliente");
                this.DescribeContact();
                ListProperty(p => p.Items).Creatable().Label("Artículos comprados");

                ShowAllInDetails();
            }
        }


        public class CompraDescriptor : CrudDescriptor<Menudeo, PagoViewModel<Menudeo>>
        {
            protected override void DescribeModel()
            {
                OnModuleMenu(Essential | Operation);

                FriendlyName("Venta");

                ObjectProperty(p => p.Vendedor).Selectable().Important("Cliente").Required();
                ListProperty(p => p.Items).Selectable().Important("Artículos comprados");
                NumericProperty(p => p.Total).Label("Total a pagar a favor");
                ListProperty(p => p.Pagos).Creatable().Important("Abonos realizados");

                VmProperty(p => p.Pendiente).ShowInDetails().AsListColumn().ReadOnly();
                VmProperty(p => p.Abonado).ShowInDetails().AsListColumn().ReadOnly();
                VmProperty(p => p.LastPagoWhen).ShowInDetails().AsListColumn().Label("Último pago").ReadOnly();
                VmProperty(p => p.LastPagoHowMuch).ShowInDetails().AsListColumn().Label("Último pago").ReadOnly();

                ShowAllInDetails();
            }
        }
    }

    namespace ViewModels
    {
        public abstract class PagoViewModel<T> : DynamicViewModel<T> where T: ModelBase, IPagable, new()
        {
            public decimal Pendiente => (Entity?.Total ?? 0m) - Abonado;
            public decimal Abonado => (Entity?.Pagos.Any() ?? false) ? Entity.Pagos.Sum(p => p.Abono) : 0m;
            public Pago? LastPago
            {
                get
                {
                    if (Pendiente == 0m || !Entity!.Pagos.Any()) return null;
                    return Entity!.Pagos.OrderBy(p => p.Timestamp).Last();
                }
            }
            public string LastPagoWhen => (Entity?.IsNew ?? true) ? "sin datos." : $"Hace {(int)(DateTime.Now - (LastPago?.Timestamp ?? Entity.Timestamp)).TotalDays} días";
            public decimal? LastPagoHowMuch => LastPago?.Abono;
        }
        public abstract class LoteViewModel : DynamicViewModel<Lote>
        {
            public int VendidosCount
            {
                get
                {
                    if (!(Entity?.Items is { } i)) return 0;
                    var c = 0;
                    foreach (IPagable j in i.Select(p => p.MenudeoParent).NotNull())
                    {
                        if (j.Debe == 0m) c++;
                    }
                    return c;
                }
            }
            public int CreditosCount
            {
                get
                {
                    if (!(Entity?.Items is { } i)) return 0;
                    var c = 0;
                    foreach (IPagable j in i.Select(p => p.MenudeoParent).NotNull())
                    {
                        if (j.Debe > 0m) c++;
                    }
                    return c;
                }
            }

            public int ExistenciasCount
            {
                get
                {
                    if (!(Entity?.Items is { } i)) return 0;
                    var c = 0;
                    foreach (var j in i)
                    {
                        if (j.MenudeoParent is null) c++;
                    }
                    return c;
                }
            }

            public LoteViewModel()
            {
                RegisterPropertyChangeBroadcast(
                    nameof(Lote.Items),
                    nameof(VendidosCount),
                    nameof(CreditosCount),
                    nameof(ExistenciasCount));
            }
        }
    }

    namespace Modules
    {
        public class ConectaModule : UiModule<ConectaService>
        {
            [InteractionItem, InteractionType(Reports), Name("Lista de productos")]
            public async void ListProducts(object? sender, EventArgs e)
            {
                if (!InputSplash.GetNew("Ingrese una búsqueda ", out string query)) return;
                await MakeListProducts(query);
            }

            [InteractionItem, InteractionType(Reports | Essential), Name("Lista agrupada de productos")]
            public async void GroupProducts(object? sender, EventArgs e) 
            {
                if (!InputSplash.GetNew("Ingrese una búsqueda ", out string query)) return;
                await MakeGroupProducts(query);
            }

            [InteractionItem, InteractionType(Reports | Essential), Name("Lista de cuentas por cobrar")]
            public async void GroupNosDebenReport(object? sender, EventArgs e)
            {
                if (!InputSplash.GetNew("Ingrese a un deudor", out string query)) return;
                await MakeNosDebenReport(query);
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
            private async Task MakeListProducts(string query)
            {
                Proteus.CommonReporter?.UpdateStatus("Generando reporte...");
                var fd = Reporting.ReportBuilder.MakeReport("Lista de productos");

                List<Item> l;
                if (query.IsEmpty())
                {
                    l = (await QE.ToListAsync(Service.All<Item>())).Where(NoPagado).ToList();
                }
                else
                {
                    l = (await QE.ToListAsync(Internal.Query(query, typeof(Lote)))).Cast<Lote>().SelectMany(p=>p.Items).Where(NoPagado).ToList();
                }

                fd.Text($"Total de artículos: {l.Count}");

                var tbl = fd.AddTable(new[] 
                {
                    new KeyValuePair<string, GridLength>("Ítem", new GridLength(3, GridUnitType.Star)),
                    new KeyValuePair<string, GridLength>("Descripción", new GridLength(4, GridUnitType.Star)),
                    new KeyValuePair<string, GridLength>("Precio", new GridLength(2, GridUnitType.Star)),
                });

                tbl.BorderBrush = Brushes.Black;
                tbl.BorderThickness = new Thickness(3);

                var flag = false;

                foreach (var j in l)
                {
                    if (j.Parent is null) continue;
                    var row = AddR(tbl, new string[]
                    {
                        j.Parent.Name,
                        string.Join(Environment.NewLine, new string?[]{ j.Parent.Description, j.Description }.NotNull()).OrNull() ?? "-",
                        CalcPrecio(j)
                    });

                    if (flag)
                    {
                        flag = false;
                        row.Background = Brushes.LightGray;
                    }
                    else { flag = true; }
                }
                try
                {
                    fd.Print("Lista de productos - Proteus");
                }
                catch
                {
                    Proteus.MessageTarget?.Stop("Hubo un problema al imprimir el reporte.");
                }
                Proteus.CommonReporter?.Done();
            }
            private async Task MakeGroupProducts(string query)
            {
                Proteus.CommonReporter?.UpdateStatus("Generando reporte...");
                var fd = Reporting.ReportBuilder.MakeReport("Lista agrupada de productos");

                List<Lote> l;
                if (query.IsEmpty())
                {
                    l = await QE.ToListAsync(Service.All<Lote>());
                }
                else
                {
                    l = (await QE.ToListAsync(Internal.Query(query, typeof(Lote)))).Cast<Lote>().ToList();
                }

                fd.Text($"Total de productos distintos: {l.Count}");
                fd.Text($"Gran total de artículos: {l.SelectMany(p=>p.Items).Where(p => p.MenudeoParent is null).Count()}");


                foreach (var j in l.NotNull())
                {
                    var itms = j.Items.Where(NoPagado).ToList();
                    if (itms.Count == 0) continue;
                    fd.Title(j.Name, 3);
                    fd.Text(string.Join(Environment.NewLine, new[] {
                        j.Description.OrNull() ?? "",
                        $"Precio de venta: {j.UnitVenta?.ToString("C") ?? "Pregunte"}",
                        $"Total de artículos: {itms.Count}"
                    }.NotNull()));
                    var ll = itms.Where(p => (!p.Description.IsEmpty()) || p.Descuento.HasValue).ToList();                    
                    if (ll.Any())
                    {
                        fd.Text($"Ítems con detalles: {ll.Count}");
                        var tbl = fd.AddTable(new[]
                        {
                            new KeyValuePair<string, GridLength>("Detalle", new GridLength(2, GridUnitType.Star)),
                            new KeyValuePair<string, GridLength>("Precio", new GridLength(1, GridUnitType.Star)),
                        });
                        tbl.BorderBrush = Brushes.Black;
                        tbl.BorderThickness = new Thickness(3);
                        var flag = false;
                        foreach (var k in ll)
                        {
                            var row = AddR(tbl, new string[]
                            {                        
                                k.Description ?? "-",
                                CalcPrecio(k)
                            });
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
                try
                {
                    fd.Print("Lista agrupada de productos - Proteus");
                }
                catch
                {
                    Proteus.MessageTarget?.Stop("Hubo un problema al imprimir el reporte.");
                }
                Proteus.CommonReporter?.Done();
            }
            private async Task MakeNosDebenReport(string query)
            {
                Proteus.CommonReporter?.UpdateStatus("Generando reporte...");
                var fd = Reporting.ReportBuilder.MakeReport("Cuentas por cobrar");

                List<Menudeo> l;
                if (query.IsEmpty())
                {
                    l = await QE.ToListAsync(Service.All<Menudeo>());
                }
                else
                {
                    l = (await QE.ToListAsync(Internal.Query(query, typeof(Menudeo)))).Cast<Menudeo>().ToList();
                }

                fd.Text($"Total de ventas registradas: {l.Count}");

                l = l.Where(m=>
                {
                    var t = m.Pagos.Any() ? m.Pagos.Sum(p => p.Abono) : 0m;
                    return t != m.Total;
                }).ToList();

                fd.Text($"Total de ventas con cuenta pendiente: {l.Count}");

                var tbl = fd.AddTable(new[]
{
                    new KeyValuePair<string, GridLength>("Fecha", new GridLength(2, GridUnitType.Star)),
                    new KeyValuePair<string, GridLength>("Cliente", new GridLength(2, GridUnitType.Star)),
                    new KeyValuePair<string, GridLength>("Artículos", new GridLength(3, GridUnitType.Star)),
                    new KeyValuePair<string, GridLength>("Total", new GridLength(1, GridUnitType.Star)),
                    new KeyValuePair<string, GridLength>("Pagado", new GridLength(1, GridUnitType.Star)),
                    new KeyValuePair<string, GridLength>("Pendiente", new GridLength(1, GridUnitType.Star)),
                    new KeyValuePair<string, GridLength>("Último pago", new GridLength(2, GridUnitType.Star)),

                });

                tbl.BorderBrush = Brushes.Black;
                tbl.BorderThickness = new Thickness(3);

                var flag = false;


                foreach (var j in l)
                {
                    var pagado = j.Pagos.Any() ? j.Pagos.Sum(p => p.Abono) : 0m;
                    var row = AddR(tbl, new string[]
                    {
                        j.Timestamp.ToString(),
                        j.Vendedor.Name,
                        string.Join(Environment.NewLine,j.Items.Select(p=>p.ToString())),
                        j.Total.ToString("C"),
                        pagado.ToString("C"),
                        (j.Total - pagado).ToString("C"),
                        j.Pagos.LastOrDefault()?.Timestamp.ToString() ?? "N/A"
                    });

                    if (flag)
                    {
                        flag = false;
                        row.Background = Brushes.LightGray;
                    }
                    else { flag = true; }
                }
                try
                {
                    fd.Print("Cuentas por cobrar - Proteus");
                }
                catch
                {
                    Proteus.MessageTarget?.Stop("Hubo un problema al imprimir el reporte.");
                }
                Proteus.CommonReporter?.Done();
            }
            private static string CalcPrecio(Item i)
            {
                return (i.Parent.UnitVenta, i.Descuento) switch
                {
                    (decimal v, null) => v.ToString("C"),
                    (decimal v, decimal d) => $"{v:C} ({v-d:C} lo menos)",
                    _ => "Pregunte"
                }; 
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
        }
    }
}
