using System;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="IsvExoneracion"/>.
    /// </summary>
    public class IsvExoneracionDescriptor : CrudDescriptor<IsvExoneracion>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="IsvExoneracion"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            Property(p => p.Id).Id("Código de exoneración");
            DateProperty(p => p.Timestamp)
                .Default(DateTime.Now)
                .Required()
                .Important("Fecha de autorización")
                .ShowInDetails()
                .AsListColumn();

            DateProperty(p => p.Void)
                .Default(DateTime.Today + TimeSpan.FromDays(365))
                .Required()
                .Important("Fecha de vencimiento")
                .ShowInDetails()
                .AsListColumn();
        }
    }
}