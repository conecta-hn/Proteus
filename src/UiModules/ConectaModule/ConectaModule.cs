using System;
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
namespace TheXDS.Proteus.Conecta
{
    namespace Crud
    {
        public class ItemDescriptor : CrudDescriptor<Item, PagoViewModel<Item>>
        {
            protected override void DescribeModel()
            {
                OnModuleMenu(Essential | Catalog);
                FriendlyName("Artículo");

                Property(p => p.Name).AsName().NotEmpty();
                Property(p => p.NumSerie).Nullable().Label("Número de serie");
                ObjectProperty(p => p.Proveedor).Selectable().Nullable();//.Creatable().Important();
                TextProperty(p => p.Description).Big().Label("Descripción del artículo");
                ListProperty(p => p.Pictures).Creatable().Label("Fotografías");
                DateProperty(p => p.Timestamp).WithTime().Label("Fecha/hora de ingreso").Default(DateTime.Now);
                NumericProperty(p => p.Qty).Range(1,int.MaxValue).Label("Cantidad de artículos").Default(1).AsListColumn();
                NumericProperty(p => p.Total).Label("Costo total");
                NumericProperty(p => p.UnitVenta).Nullable().Label("Precio de venta unitario");

                ListProperty(p => p.Pagos).Creatable().Important("Abonos realizados al proveedor");
                VmProperty(p => p.Pendiente).ShowInDetails().AsListColumn().ReadOnly();
                VmProperty(p => p.Abonado).ShowInDetails().AsListColumn().ReadOnly();
                VmProperty(p => p.LastPagoWhen).ShowInDetails().AsListColumn().Label("Último pago").ReadOnly();
                VmProperty(p => p.LastPagoHowMuch).ShowInDetails().AsListColumn().Label("Último pago").ReadOnly();
                ShowAllInDetails();
            }
        }

        public class ItemPictureDescriptor : CrudDescriptor<ItemPicture>
        {
            protected override void DescribeModel()
            {
                FriendlyName("Imagen de artículo");
                TextProperty(p => p.Path).TextKind(TextKind.PicturePath).Label("Imagen");
                TextProperty(p => p.Notes).Big().Label("Notas");
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

        public class CompraDescriptor : CrudDescriptor<Compra, PagoViewModel<Compra>>
        {
            protected override void DescribeModel()
            {
                OnModuleMenu(Essential | Operation);

                ObjectProperty(p => p.Comprador).Selectable().Important().Required();
                ObjectProperty(p => p.Item).Selectable().Important().Required();
                NumericProperty(p => p.Qty).Range(1, int.MaxValue).Important("Cantidad").Default(1);
                NumericProperty(p => p.Total).Label("Total a pagar");
                ListProperty(p => p.Pagos).Creatable().Important("Abonos realizados");
                VmProperty(p => p.Pendiente).ShowInDetails().AsListColumn().ReadOnly();
                VmProperty(p => p.Abonado).ShowInDetails().AsListColumn().ReadOnly();
                VmProperty(p => p.LastPagoWhen).ShowInDetails().AsListColumn().Label("Último pago").ReadOnly();
                VmProperty(p => p.LastPagoHowMuch).ShowInDetails().AsListColumn().Label("Último pago").ReadOnly();

                ShowAllInDetails();
            }
        }

        public class ClienteDescriptor : CrudDescriptor<Cliente>
        {
            protected override void DescribeModel()
            {
                OnModuleMenu(Essential | AdminTool);

                Property(p => p.Name).AsName("Nombre del cliente");
                this.DescribeContact();
                ListProperty(p => p.Compras).Creatable().Label("Compras realizadas");

                ShowAllInDetails();
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
                    if (Pendiente == 0m) return null;
                    return Entity!.Pagos.OrderBy(p => p.Timestamp).Last();
                }
            }
            public TimeSpan? LastPagoWhen => DateTime.Now - (LastPago?.Timestamp ?? Entity?.Timestamp);
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
