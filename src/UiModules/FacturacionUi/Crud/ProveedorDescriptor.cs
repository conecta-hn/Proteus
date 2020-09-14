using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    public class ProveedorDescriptor : CrudDescriptor<Proveedor>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.Catalog);
            FriendlyName("Proveedor");

            Property(p => p.Name).AsName("Nombre del proveedor").NotEmpty();

            Property(p => p.WebPage).Label("Página Web");

            ListProperty(p => p.Contactos)
                .Editable()
                .Creatable()
                .Label("Contactos")
                .Required();

            ListProperty(p => p.Productos)
                .Selectable()
                .Creatable()
                .Label("Productos");

            this.DescribeAddress();

            this.DescribeContact();
        }
    }
}