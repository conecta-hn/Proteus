/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using static TheXDS.MCART.Types.Extensions.StringExtensions;

namespace TheXDS.Proteus.Crud
{
    public class AvisoCrudDescriptor : CrudDescriptor<Aviso>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Aviso");

            DateProperty(p => p.Timestamp)
                .AsListColumn()
                .Default(DateTime.Now)
                .ShowInDetails()
                .Label("Fecha de creación")
                .ReadOnly();

            TextProperty(p => p.Header)
                .MaxLength(150)
                .Validator<Aviso>(CheckTitle)
                .Label("Título")
                .ShowInDetails()
                .AsListColumn()
                .Required();

            TextProperty(p => p.Body)
                .MaxLength(4096)
                .TextKind(TextKind.Big)
                .Validator<Aviso>(CheckBody)
                .Label("Contenido")
                .ShowInDetails()
                .Required();

            BeforeSave(SetCreationTime);

            CustomAction("Vista previa", o => Proteus.MessageTarget?.Show(o.Header, o.Body));
        }

        private IEnumerable<ValidationError> CheckBody(Aviso m, PropertyInfo p)
        {
            if (m.Body.IsEmpty()) yield return new ValidationError(p, "Un aviso debe contener texto.");
            if (m.Body.Length > 4096) yield return new ValidationError(p, "El aviso es demasiado largo. El límite es de 4 KB");
        }

        private IEnumerable<ValidationError> CheckTitle(Aviso m, PropertyInfo p)
        {
            if (m.Header.IsEmpty()) yield return new ValidationError(p, "Se necesita un título");
            if (m.Header.Length > 150) yield return new ValidationError(p, "El título del aviso es demasiado largo. El límite es de 150 caracteres.");
        }

        private void SetCreationTime(Aviso obj)
        {
            obj.Timestamp = DateTime.Now;
        }
    }
}