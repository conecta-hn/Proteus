using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="Cajero"/>.
    /// </summary>
    public class CajeroDescriptor : CrudDescriptor<Cajero>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="Cajero"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.Settings);
            LinkProperty<User>(p => p.UserId)
                .Source(UserService.InteractiveUsers)
                .Important("Usuario")
                .Required();
            NumericProperty(p => p.OptimBalance)
                .Range(0m, 1000000000m)
                //.Format("L. {0}")
                .Format("C")
                .ShowInDetails()
                .AsListColumn("C")
                .Label("Fondo de caja asignado");
        }
    }
}