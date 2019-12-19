/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Facturacion.Models;
using TheXDS.Proteus.Models.Base;
using System;

namespace TheXDS.Proteus.Facturacion.Crud
{
    public class CaiDescriptor : CrudDescriptor<Cai>
    {
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
                .Nullable();

            ListProperty(p => p.Rangos).Creatable().Label("Rangos de facturación").ShowInDetails();

            BeforeSave(MakeIdUpperCase);
        }

        private void MakeIdUpperCase(Cai arg1, ModelBase _)
        {
            arg1.Id = arg1.Id.ToUpper();
        }
    }
}