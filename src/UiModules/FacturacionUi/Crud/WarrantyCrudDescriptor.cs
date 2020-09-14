using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="Warranty"/>.
    /// </summary>
    public class WarrantyCrudDescriptor : CrudDescriptor<Warranty>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="Warranty"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            CanCreate(false);
            OnModuleMenu(Annotations.InteractionType.AdminTool);
            FriendlyName("Garantía orotgada");

            DateProperty(p => p.Void).Label("Fecha de inicio");
            ObjectProperty(p => p.Cliente).Selectable().Required();
            DateProperty(p => p.Void).Label("Fecha de expiración");

            ListProperty(p => p.Items)
                .Selectable()
                .Required()
                .Label("Items cubiertos")
                .ReadOnly();

            ShowAllInDetails();
            AllListColumn();
        }
    }
}