/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Facturacion.Models;
using TheXDS.Proteus.Misc;
using System;
using System.Drawing;
using System.Drawing.Printing;

namespace TheXDS.Proteus.Facturacion.Crud
{
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

            var img = Internal.MakeBarcode(obj);

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