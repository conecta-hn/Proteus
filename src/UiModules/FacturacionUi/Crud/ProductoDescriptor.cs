using System;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="Producto"/>.
    /// </summary>
    public class ProductoDescriptor : CrudDescriptor<Producto>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="Producto"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            this.DescribeFacturable();
            TextProperty(p => p.Description)
                .Big()
                .Nullable()
                .Label("Descripción")
                .ShowInDetails();
            
            TextProperty(p => p.Picture)
                .TextKind(TextKind.PicturePath)
                .Nullable()
                .Label("Imagen del producto")
                .ShowInDetails();
            
            NumericProperty(p => p.StockMin)
                .Positive()
                .Nullable()
                .ShowInDetails()
                .Label("Stock mínimo");
            
            NumericProperty(p => p.StockMax)
                .Positive()
                .Nullable()
                .ShowInDetails()
                .Label("Stock máximo");
            
            NumericProperty(p => p.ExpiryDays)
                .Positive()
                .Nullable()
                .ShowInDetails()
                .Label("Producto expirable (días de duración)");

            ListProperty(p => p.Labels)
                .Selectable()
                .Creatable()
                .ShowInDetails()
                .Label("Etiquetas de tipo de inventario");

            BeforeSave(ChkStocks);
        }

        private void ChkStocks(Producto arg1, ModelBase arg2)
        {
            if (arg1.StockMin is { } min && arg1.StockMax is { } max && min > max)
            throw new Exception("La cantidad de stock mínimo debe ser menor a la de stock máximo.");
        }
    }
}