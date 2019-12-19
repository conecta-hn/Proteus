/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/


using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Inventario.Models;

namespace TheXDS.Proteus.Crud
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