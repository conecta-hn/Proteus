﻿using System;
using System.Collections.Generic;
using System.Text;
using TheXDS.MCART.Types.Base;
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
                .Nullable();

            ListProperty(p => p.Rangos).Creatable().Label("Rangos de facturación").ShowInDetails();

            BeforeSave(MakeIdUpperCase);
        }

        private void MakeIdUpperCase(Cai arg1, ModelBase arg2)
        {
            arg1.Id = arg1.Id.ToUpper();
        }
    }

    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="CajaOp"/>.
    /// </summary>
    public class CajaOpDescriptor : CrudDescriptor<CajaOp>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="CajaOp"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            FriendlyName("Sesión de caja");

            ObjectProperty(p => p.Estacion).Selectable().Label("Estación");
            ObjectProperty(p => p.Cajero).Selectable();
            NumericProperty(p => p.OpenBalance).Label("Balance de apertura");

            //CustomAction("Cerrar sesión de caja", OnCloseSession);
        }

        private void OnCloseSession(CajaOp obj, NotifyPropertyChangeBase vm)
        {
            //TODO: ventana de input
        }
    }
}