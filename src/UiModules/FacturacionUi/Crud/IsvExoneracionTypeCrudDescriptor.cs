using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="IsvExoneracionType"/>.
    /// </summary>
    public class IsvExoneracionTypeCrudDescriptor : CrudDescriptor<IsvExoneracionType>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="IsvExoneracionType"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            FriendlyName("Tipo de exoneración de ISV");

            Property(p => p.ApplyRule).Label("Aplicación de regla").ShowInDetails();
            ListProperty(p => p.RuleItems).Selectable().Label("Categorías aplicables").ShowInDetails();
        }
    }
}