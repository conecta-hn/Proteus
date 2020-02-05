using System;
using System.Drawing;
using System.Drawing.Printing;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Misc;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="Cliente"/>.
    /// </summary>
    public class ClienteDescriptor : CrudDescriptor<Cliente>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="Cliente"/>.
        /// </summary>
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

            ListProperty(p => p.Exoneraciones)
                .Creatable()
                .Label("Exoneraciones de ISV autorizadas")
                .ShowInDetails();

            ListProperty(p => p.Facturas).Creatable().ShowInDetails();
            ListProperty(p => p.Cotizaciones).Creatable().ShowInDetails();

            ListProperty(p => p.Credits)
                .Creatable()
                .Label("Créditos otorgados al cliente")
                .ShowInDetails();

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

    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="FacturaXCobrar"/>.
    /// </summary>
    public class FacturaXCobrarDescriptor : CrudDescriptor<FacturaXCobrar>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="FacturaXCobrar"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            FriendlyName("Cuenta por cobrar");
            DateProperty(p => p.Timestamp).Timestamp().AsListColumn().ShowInDetails();
            ObjectProperty(p => p.Cliente).Selectable().Required().AsListColumn().ShowInDetails();
            ObjectProperty(p => p.Parent).Selectable().Label("Factura a pagar").Required().AsListColumn().ShowInDetails();
            NumericProperty(p => p.Total).Positive().AsListColumn().ShowInDetails();
            ListProperty(p => p.Abonos).Creatable();
            Property(p => p.IsPaid)
                .Label("Cuenta pagada")
                .ShowInDetails()
                .AsListColumn()
                .Hidden();
            BeforeSave(SetIsPaid);
        }

        private void SetIsPaid(FacturaXCobrar arg1, ModelBase arg2)
        {
            arg1.IsPaid = arg1.Pending == 0m;
        }
    }

    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="Abono"/>.
    /// </summary>
    public class AbonoDescriptor : CrudDescriptor<Abono>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="Abono"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            DateProperty(p => p.Timestamp).Timestamp().AsListColumn().ShowInDetails();
            NumericProperty(p => p.Amount).Positive().Label("Monto del pago").AsListColumn("C").ShowInDetails();
        }
    }
}