using System;
using System.Collections.Generic;
using System.Text;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="Cai"/>.
    /// </summary>
    public class CaiDescriptor : CrudDescriptor<Cai>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="Cai"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            FriendlyName("CAI");

            TextProperty(p => p.Id)
                .Mask("AAAAAA-AAAAAA-AAAAAA-AAAAAA-AAAAAA-AA")
                .Id("Código de Autorización de impresión");

            Property(p => p.Timestamp)
                .Important("Fecha de autorización")
                .Default(DateTime.Now);

            Property(p => p.Void)
                .Label("Fecha de vencimiento")
                .Default(DateTime.Now + TimeSpan.FromDays(365));

            ListProperty(p => p.Rangos).Creatable().Label("Rangos de facturación").ShowInDetails();

            BeforeSave(MakeIdUpperCase);
        }

        private void MakeIdUpperCase(Cai arg1, ModelBase arg2)
        {
            arg1.Id = arg1.Id.ToUpper();
        }
    }
}