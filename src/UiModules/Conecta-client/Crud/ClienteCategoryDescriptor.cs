/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Facturacion.Models;

namespace TheXDS.Proteus.Facturacion.Crud
{
    public class ClienteCategoryDescriptor : CrudDescriptor<ClienteCategory>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.Catalog);
            FriendlyName("Categoría de cliente");

            Property(p => p.Name).AsName();

            Property(p => p.RequireRTN).Label("Requerir RTN de manera obligatoria");

            ListProperty(p => p.Members).Selectable().Label("Miembros");
            ObjectProperty(p => p.AltPrecios).Selectable().Label("Tabla de precios alternativa");
        }
    }
}