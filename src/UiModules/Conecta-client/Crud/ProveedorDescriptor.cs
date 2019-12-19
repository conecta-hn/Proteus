/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Inventario.Models;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Crud
{
    public class ProveedorDescriptor : CrudDescriptor<Proveedor>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Proveedor");

            Property(p => p.Name).AsName("Nombre del proveedor").NotEmpty();

            Property(p => p.WebPage).Label("Página Web");

            ListProperty(p => p.Contactos)
                .Editable()
                .Creatable()
                .Label("Contactos")
                .Required();

            this.DescribeAddress();

            this.DescribeContact();
        }
    }
}
