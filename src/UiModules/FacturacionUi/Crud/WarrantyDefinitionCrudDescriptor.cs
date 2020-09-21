using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="WarrantyDefinition"/>.
    /// </summary>
    public class WarrantyDefinitionCrudDescriptor : CrudDescriptor<WarrantyDefinition>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="WarrantyDefinition"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(Annotations.InteractionType.Settings);
            FriendlyName("Definición de garantía");
            Property(p => p.Name).AsName();
            TextProperty(p => p.TemplatePath).TextKind(TextKind.FilePath).Label("Ruta de plantilla de garantía");
            NumericProperty(p => p.WarrantyLength).Positive().Nullable().Label("Longitud de garantía").AsListColumn();
            Property(p => p.Unit).Label("Unidad").Nullable().AsListColumn();
            ShowAllInDetails();
            
        }
    }

}