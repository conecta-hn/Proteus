using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TheXDS.MCART;
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
using OfficeOpenXml;
using static TheXDS.MCART.Types.Extensions.EnumerableExtensions;
using static TheXDS.MCART.Types.Extensions.FlowDocumentExtensions;
using static TheXDS.MCART.Types.Extensions.StringExtensions;
using static TheXDS.Proteus.Annotations.InteractionType;
using QE = System.Data.Entity.QueryableExtensions;
using System.IO;
using Microsoft.Win32;
using TheXDS.Proteus.ConectaModule.Pages;

namespace TheXDS.Proteus.Conecta
{
    namespace Crud
    {
        // Inventario
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
                VmProperty(p => p.CreditosCount).Label("Créditos abiertos").OnlyInDetails();
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
        public class InversorDescriptor : CrudDescriptor<Inversor, InversorViewModel>
        {
            protected override void DescribeModel()
            {
                OnModuleMenu(Essential | AdminTool);

                Property(p => p.Name).AsName("Nombre del inversor");
                this.DescribeContact();

                Property(p => p.InvTotal).Label("Inversión total adeudada").Format("C").OnlyInDetails();
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


        // Actividades

        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="Actividad"/>.
        /// </summary>
        public class ActividadDescriptor : CrudDescriptor<Actividad>
        {
            /// <summary>
            /// Describe las propiedades Crud para el modelo
            /// <see cref="Actividad"/>.
            /// </summary>
            protected override void DescribeModel()
            {
                OnModuleMenu(AdminTool | Essential);

                DateProperty(p => p.Timestamp)
                    .WithTime()
                    .Important("Fecha / hora de inicio");

                DateProperty(p => p.Void)
                    .WithTime()
                    .Nullable()
                    .ShowInDetails()
                    .AsListColumn()
                    .Label("Fecha / hora de final");

                Property(p => p.Name).AsName("Descripción de la actividad");

                ListProperty(p => p.Items)
                    .Creatable()
                    .Required()
                    .Label("Puntos de actividad")
                    .ShowInDetails();

                TextProperty(p => p.Description)
                    .Big()
                    .Required()
                    .NotEmpty()
                    .Label("Notas")
                    .ShowInDetails();
            }
        }

        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="ActividadItem"/>.
        /// </summary>
        public class ActividadItemDescriptor : CrudDescriptor<ActividadItem>
        {
            /// <summary>
            /// Describe las propiedades Crud para el modelo
            /// <see cref="ActividadItem"/>.
            /// </summary>
            protected override void DescribeModel()
            {
                Property(p => p.Name).AsName();
                Property(p => p.RawValue).Important("Ingreso / gasto");
                TextProperty(p => p.Description)
                    .Big()
                    .Required()
                    .NotEmpty()
                    .Label("Notas")
                    .ShowInDetails();
            }
        }


        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="Asistencia"/>.
        /// </summary>
        public class AsistenciaDescriptor : CrudDescriptor<Asistencia>
        {
            /// <summary>
            /// Describe las propiedades Crud para el modelo
            /// <see cref="Asistencia"/>.
            /// </summary>
            protected override void DescribeModel()
            {
                OnModuleMenu(AdminTool | Essential);
                DateProperty(p => p.Timestamp)
                    .WithTime()
                    .Important("Fecha / hora de entrada");

                DateProperty(p => p.Void)
                    .WithTime()
                    .Nullable()
                    .Default(null!)
                    .ShowInDetails()
                    .AsListColumn()
                    .Label("Fecha / hora de salida");

                ObjectProperty(p => p.Empleado)
                    .Selectable()
                    .Required()
                    .Important();
            }
        }


        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="Empleado"/>.
        /// </summary>
        public class EmpleadoDescriptor : CrudDescriptor<Empleado>
        {
            /// <summary>
            /// Describe las propiedades Crud para el modelo
            /// <see cref="Empleado"/>.
            /// </summary>
            protected override void DescribeModel()
            {
                OnModuleMenu(AdminTool | Essential);
                Property(p => p.Name).AsName("Nombre del empleado");
                this.DescribeContact();
            }
        }
    }


    namespace Modules
    {
        [Name("Inventario")]
        public class ConectaModule : UiModule<ConectaService>
        {
            

            [InteractionItem, InteractionType(Reports), Essential, Name("Listas de productos")]
            public async void ListProducts(object? sender, EventArgs e)
            {
                Host.OpenPage<ListReportGenPage>();
            }

            [InteractionItem, InteractionType(Reports), Essential, Name("Cuentas por cobrar")]
            public async void NosDebenReport(object? sender, EventArgs e)
            {
                if (!InputSplash.GetNew("Ingrese a un deudor", out string query)) return;
                await MakeNosDebenReport(query);
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

                l = l.Where(m =>
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

        [Name("Actividades")]
        public class ConectaActModule : UiModule<ConectaActService>
        {
        }
    }
}
