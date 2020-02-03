using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Types.Base;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.FacturacionUi.Modules;
using TheXDS.Proteus.FacturacionUi.ViewModels;
using TheXDS.Proteus.Misc;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    public class CaiDescriptor : CrudDescriptor<Cai>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            FriendlyName("CAI");

            TextProperty(p => p.Id)
                .Mask("AAAAAA-AAAAAA-AAAAAA-AAAAAA-AAAAAA-AA")
                .Id("Código de Autorización de impresión");

            Property(p => p.Timestamp)
                .Important("Fecha de autorización")
                .Default(DateTime.Now);

            Property(p => p.Void)
                .Label("Fecha de vencimiento")
                .Nullable();

            ListProperty(p => p.Rangos).Creatable().Label("Rangos de facturación").ShowInDetails();

            BeforeSave(MakeIdUpperCase);
        }

        private void MakeIdUpperCase(Cai arg1, ModelBase arg2)
        {
            arg1.Id = arg1.Id.ToUpper();
        }
    }

    public class CaiRangoDescriptor : CrudDescriptor<CaiRango>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Rango de facturación");
            NumericProperty(p => p.NumLocal).Range(0, 999).Format("000").Important("Local");
            NumericProperty(p => p.NumCaja).Range(0, 999).Format("000").Important("Caja");
            NumericProperty(p => p.NumDocumento).Range(0, 99).Format("00").Important("Documento");
            NumericProperty(p => p.RangoInicial).Range(0, 99999999).Format("00000000").Important("Rango inicial");
            NumericProperty(p => p.RangoFinal).Range(0, 99999999).Format("00000000").Important("Rango Final");
        }
    }
    public class CajaOpDescriptor : CrudDescriptor<CajaOp>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Sesión de caja");

            ObjectProperty(p => p.Estacion).Selectable().Label("Estación");
            ObjectProperty(p => p.Cajero).Selectable();
            NumericProperty(p => p.OpenBalance).Label("Balance de apertura");

            //CustomAction("Cerrar sesión de caja", OnCloseSession);
        }

        private void OnCloseSession(CajaOp obj, NotifyPropertyChangeBase vm)
        {
            //TODO: ventana de input
        }
    }
    public class CajeroDescriptor : CrudDescriptor<Cajero>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            LinkProperty<User>(p => p.UserId)
                .Source(UserService.InteractiveUsers)
                .Important("Usuario")
                .Required();
            NumericProperty(p => p.OptimBalance)
                .Range(0m, 1000000000m)
                .Format("C")
                .ShowInDetails()
                .AsListColumn("C")
                .Label("Fondo de caja asignado");
        }
    }
    public class ClienteCategoryDescriptor : CrudDescriptor<ClienteCategory>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.Catalog);
            FriendlyName("Categoría de cliente");

            Property(p => p.Name)
                .AsName();

            Property(p => p.RequireRTN)
                .Label("Requerir RTN de manera obligatoria");

            ListProperty(p => p.Members)
                .Selectable()
                .Label("Miembros");

            ObjectProperty(p => p.AltPrecios)
                .Selectable()
                .Creatable()
                .Label("Tabla de precios alternativa");
        }
    }
    public class ClienteDescriptor : CrudDescriptor<Cliente>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            Property(p => p.Name).AsName("Nombre del cliente");
            this.DescribeContact();
            Property(p => p.Timestamp)
                .Default(DateTime.Now)
                .AsListColumn()
                .ShowInDetails()
                .Label("Fecha de creación")
                .ReadOnly();

            TextProperty(p => p.Rtn)
                .Mask("0000-0000-000000")
                .Nullable()
                .Label("R. T. N.")
                .ShowInDetails()
                .AsListColumn();

            ObjectProperty(p => p.Category)
                .Nullable()
                .Label("Categoría")
                .ShowInDetails()
                .AsListColumn();

            CustomAction("Generar Carnet de cliente", OnGenerateCard);
        }

        private void OnGenerateCard(Cliente obj)
        {
            if (obj.IsNew)
            {
                Proteus.MessageTarget?.Show("Guarde al cliente primero.");
                return;
            }

            var img = AppInternal.MakeBarcode(obj);

            var pd = new PrintDocument();
            pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", 100, 77);

            pd.DefaultPageSettings.Landscape = false;
            pd.PrintPage += (_, g) =>
            {
                int printHeight = 450;
                int printWidth = 400;
                int leftMargin = 20;
                int rightMargin = 0;

                img.RotateFlip(RotateFlipType.Rotate90FlipNone);

                g.Graphics.DrawImage(img, new Rectangle(leftMargin, rightMargin, printWidth, printHeight));
            };
            pd.Print();
        }
    }
    public class CotizacionDescriptor : CrudDescriptor<Cotizacion>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            FriendlyName("Cotización");

            DateProperty(p => p.Timestamp)
                .WithTime()
                .Default(DateTime.Now)
                .Label("Fecha de creación")
                .AsListColumn()
                .ShowInDetails()
                .ReadOnly();

            DateProperty(p => p.Void)
                .WithTime()
                .Nullable()
                .Default(DateTime.Now)
                .Label("Fecha de vencimiento")
                .AsListColumn()
                .ShowInDetails();

            ObjectProperty(p => p.Cliente)
                .Selectable()
                .Nullable();

            ListProperty(p => p.Items).Creatable();
            Property(p => p.SubTotal).Label("Sub total").ReadOnly();
            Property(p => p.SubTGravable).Label("Sub total gravable").ReadOnly();
            Property(p => p.SubTGravado).Label("Sub total gravado").ReadOnly();
            Property(p => p.SubTFinal).Label("Sub total final").ReadOnly();
            NumericProperty(p => p.Descuentos).Range(0m, decimal.MaxValue);
            NumericProperty(p => p.OtrosCargos).Range(0m, decimal.MaxValue);
            Property(p => p.Total).Label("Total a pagar").ReadOnly();
            TextProperty(p => p.Notas).Big();
            CustomAction("Imprimir", OnPrint);
            CustomAction("Facturar", OnFacturar);
        }

        private void OnFacturar(Cotizacion obj)
        {
            FacturacionModule.DefaultFacturaLauncher.Command.Execute((Factura)obj);
        }

        private void OnPrint(Cotizacion obj)
        {
        }
    }
    public class EstacionDescriptor : CrudDescriptor<Estacion>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            FriendlyName("Estación de facturación");

            this.DescribeEstacion();
            ListProperty(p => p.RangosAsignados)
                .Selectable()
                .Source(FacturaService.UnassignedRangos)
                .Label("Rangos de facturación asignados");
            NumericProperty(p => p.SecondScreen).Range(2, 255).Nullable().Label("Pantalla secundaria");
            NumericProperty(p => p.MinFacturasAlert).Range(0, 99999999).Nullable().Label("Nivel de alerta de mínimo de facturas");
        }
    }
    public class FacturaDescriptor : CrudDescriptor<Factura, FacturaCrudViewModel>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);

            VmProperty(p => p.FacturaNumber).OnlyInDetails("Número de factura");
            DateProperty(p => p.Timestamp)
                .WithTime()
                .Default(DateTime.Now)
                .Label("Fecha de creación")
                .AsListColumn()
                .ShowInDetails()
                .ReadOnly();
            ObjectProperty(p => p.Cliente).Selectable().Important().Required();
            ListProperty(p => p.Items).Creatable();
            Property(p => p.SubTotal).Label("Sub total").ReadOnly();
            Property(p => p.SubTGravable).Label("Sub total gravable").ReadOnly();
            Property(p => p.SubTGravado).Label("Sub total gravado").ReadOnly();
            Property(p => p.SubTFinal).Label("Sub total final").ReadOnly();
            NumericProperty(p => p.Descuentos).Range(0m, decimal.MaxValue);
            NumericProperty(p => p.OtrosCargos).Range(0m, decimal.MaxValue);
            Property(p => p.Total).Label("Total a pagar").ReadOnly();
            ListProperty(p => p.Payments).Creatable().Label("Pagos");
            Property(p => p.Paid).ReadOnly();
            TextProperty(p => p.Notas).Big();
            Property(p => p.Impresa).Label("Impresa").ShowInDetails().ReadOnly();
            Property(p => p.Nula).Label("Nula").ShowInDetails().ReadOnly();
            CustomAction("Imprimir factura", OnPrint);
            CustomAction("Anular factura", OnNullify);

            BeforeSave(SetCorrel).Then(SetCajaOp);
        }

        private void OnPrint(Factura obj)
        {
            if (obj.Impresa)
            {
                FacturaService.PrintFactura(obj, App.Module<FacturacionModule>().Interactor);
            }
            else
            {
                FacturaService.AddFactura(obj, true, App.Module<FacturacionModule>().Interactor);
            }
            CurrentEditor.SaveCommand.Execute(obj);
        }

        private async void OnNullify(Factura obj)
        {
            obj.Nula = true;
            CurrentEditor.SaveCommand.Execute(obj);
        }

        private void SetCajaOp(Factura obj)
        {
            obj.Parent = FacturaService.GetCajaOp;
        }

        private void SetCorrel(Factura factura)
        {
            var r = FacturaService.CurrentRango;
            factura.CaiRangoParent = r;
            factura.Correlativo = FacturaService.NextCorrel(r) ?? 0;
        }
    }
    public class ItemFacturaDescriptor : CrudDescriptor<ItemFactura, ItemFacturaCrudViewModel>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Ítem de factura");
            VmObjectProperty(p => p.ActualItem).Selectable().Label("Ítem");
            ObjectProperty(p => p.Item).AsListColumn().OnlyInDetails();
            NumericProperty(p => p.Qty).Range(1, 1000000).Important("Cantidad");
            VmNumericProperty(p => p.Precio).Range(decimal.Zero, decimal.MaxValue).Important("Precio de venta");
            VmNumericProperty(p => p.Descuento).Range(decimal.Zero, decimal.MaxValue).Important("Descuento individual otorgado");
            VmNumericProperty(p => p.Isv).Range(0, 100).Nullable().Label("ISV");
        }
    }
    public class PaqueteDescriptor : CrudDescriptor<Paquete, PaqueteCrudViewModel>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            this.DescribeFacturable();
            ListProperty(p => p.Children)
                .Selectable()
                .Label("Ítems contenidos en la promoción");
            DateProperty(p => p.ValidFrom)
                .Required()
                .Label("Válida desde")
                .Default(DateTime.Now);
            DateProperty(p => p.Void)
                .Nullable()
                .Label("Fecha de vencimiento")
                .AsListColumn()
                .ShowInDetails()
                .Default(DateTime.Now.AddMonths(1));

            NumericProperty(p => p.Precio).Hidden();

            VmProperty(p => p.Price)
                .Important("Precio de venta").RadioSelectable();
            VmProperty(p => p.Percent)
                .Important("Porcentaje de descuento").RadioSelectable();

        }
    }
    public class PaymentDescriptor : CrudDescriptor<Payment>
    {
        protected override void DescribeModel()
        {
            Property(p => p.Source);
            NumericProperty(p => p.Amount).Range(decimal.Zero, decimal.MaxValue).Important("Monto");
        }
    }
    public class ProductoDescriptor : CrudDescriptor<Producto>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            this.DescribeFacturable();
            //ListProperty(p => p.Consumos).Creatable().Label("Consumos de inventario");
        }
    }
    public class ServicioDescriptor : CrudDescriptor<Servicio>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            this.DescribeFacturable();
        }
    }
    public class TablaPrecioDescriptor : CrudDescriptor<TablaPrecio>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            Property(p => p.Name).AsName();
            ListProperty(p => p.Items).Creatable();
            ListProperty(p => p.AppliedTo).Selectable().Label("Aplicado a las categorías");
        }
    }
    public class TablaPrecioItemDescriptor : CrudDescriptor<TablaPrecioItem>
    {
        protected override void DescribeModel()
        {
            ObjectProperty(p => p.Item).Selectable();
            this.DescribeValuable();
        }
    }









    public static class PropertyDescriptorShortcuts
    {
        [Sugar]
        public static void DescribeFacturable<T>(this CrudDescriptor<T> descriptor) where T : Facturable, new()
        {
            descriptor.CanDelete(p => !p.Instances.Any());
            descriptor.Property(p => p.Name).AsName();
            descriptor.NumericProperty(p => p.Isv)
                .Range(0, 1)
                .Nullable()
                .Label("ISV")
                .ShowInDetails()
                .AsListColumn();

            descriptor.NumericProperty(p => p.Precio)
                .Range(decimal.Zero, decimal.MaxValue)
                .Important("Precio de venta");
        }
    }
}