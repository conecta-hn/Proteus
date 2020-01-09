﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Conecta.Api;
using TheXDS.Proteus.Conecta.Models;
using TheXDS.Proteus.Conecta.ViewModels;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Plugins;
using static TheXDS.Proteus.Annotations.InteractionType;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Dialogs;
using System.IO;
using Microsoft.Win32;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Types.Extensions;

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
                ObjectProperty(p => p.MenudeoParent).Selectable().Important("En menudeo").Nullable();
            }
        }

        public class LoteDescriptor : CrudDescriptor<Lote>
        {
            protected override void DescribeModel()
            {
                OnModuleMenu(Essential | Catalog);
                FriendlyName("Lote de artículos");

                Property(p => p.Name).AsName().NotEmpty();
                ObjectProperty(p => p.Proveedor).Selectable().Nullable();
                ListProperty(p => p.Inversion).Creatable().Label("Inversiones");
                TextProperty(p => p.Description).Big().Label("Detalles");
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

                Property(p => p.Name).AsName("Nombre del vendedor");
                this.DescribeContact();
                ListProperty(p => p.Items).Creatable().Label("Artículos en menudeo");

                ShowAllInDetails();
            }
        }


        public class CompraDescriptor : CrudDescriptor<Menudeo, PagoViewModel<Menudeo>>
        {
            protected override void DescribeModel()
            {
                OnModuleMenu(Essential | Operation);

                ObjectProperty(p => p.Vendedor).Selectable().Important().Required();
                ListProperty(p => p.Items).Selectable().Important("Artículos en menudeo");
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
    }

    namespace Modules
    {
        public class ConectaModule : UiModule<ConectaService>
        {
        }
    }
}
