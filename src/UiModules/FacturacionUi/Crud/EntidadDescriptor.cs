using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="Entidad"/>.
    /// </summary>
    public class EntidadDescriptor : CrudDescriptor<Entidad>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="Entidad"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.Settings);
            FriendlyName("Entidad de facturación");
            Property(p => p.Name).AsName();
            TextProperty(p => p.Id).Mask("0000-0000-000000").Id("R.T.N.");
            Property(p => p.Banner).Nullable();
            this.DescribeAddress();
            this.DescribeContact();
        }
    }
}