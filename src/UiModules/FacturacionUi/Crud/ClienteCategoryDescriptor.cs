using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="ClienteCategory"/>.
    /// </summary>
    public class ClienteCategoryDescriptor : CrudDescriptor<ClienteCategory>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="ClienteCategory"/>.
        /// </summary>
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
}