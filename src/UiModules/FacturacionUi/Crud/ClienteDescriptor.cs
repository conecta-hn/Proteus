using System;
using System.Drawing;
using System.Drawing.Printing;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Misc;
using TheXDS.Proteus.Models;

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

            Property(p => p.CanCredit).Label("Permitir venta al crédito");
            NumericProperty(p => p.CreditLimit)
                .Range(0, decimal.MaxValue)
                .Label("Límite de crédito")
                .Nullable().ShowInDetails();

            Property(p => p.CanPrepay).Label("Permitir prepago");


            ListProperty(p => p.Debits)
                .Creatable()
                .Label("Débitos activos del cliente")
                .ShowInDetails();

            Property(p => p.Prepaid).Label("Créditos del cliente")
                .ShowInDetails()
                .ReadOnly("C");

            Property(p => p.TotalCredits)
                .Label("Créditos totales otorgados")
                .ShowInDetails().ReadOnly();
            Property(p => p.TotalCredits)
                .Label("Créditos adeudados actualmente")
                .ShowInDetails()
                .ReadOnly();
            Property(p => p.TotalCredits)
                .Label("Crédito disponible")
                .ShowInDetails()
                .ReadOnly();

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
}