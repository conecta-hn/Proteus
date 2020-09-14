using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    public class ProveedorContactoDescriptor : CrudDescriptor<ProveedorContacto>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Contacto de proveedor");

            Property(p => p.Name).AsName("Nombre del contacto");
            Property(p => p.Cargo).Label("Cargo del contacto").Nullable();

            this.DescribeContact();
        }
    }
}