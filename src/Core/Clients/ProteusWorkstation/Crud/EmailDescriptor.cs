/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System.Reflection;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Crud
{
    public class EmailDescriptor : CrudDescriptor<Email>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Correo electrónico");

            TextProperty(p => p.Address)
                .Validator<Email>(IsEmail)
                .Label("Dirección de correo")
                .Icon("📧")
                .AsListColumn()
                .ShowInDetails()
                .Required();
        }

        private IEnumerable<ValidationError> IsEmail(Email entity, PropertyInfo p)
        {
            if (entity?.Address is null) yield return "Entidad nula.";
            var tokens = entity.Address.Split('@');
            if (tokens.Length != 2 || !tokens[1].Contains("."))
            {
                yield return new ValidationError(p, "La dirección de correo no es válida.");
                yield break;
            }
            foreach( var j in tokens[0].ToCharArray())
                if (!"abcdefghijklmnopqrstuvwxyz1234567890_-.".Contains(j.ToString())) yield return new ValidationError(p,"El nombre de la dirección contiene caracteres ilegales.");
            foreach (var j in tokens[1].ToCharArray())
                if (!"abcdefghijklmnopqrstuvwxyz1234567890_-.".Contains(j.ToString())) yield return new ValidationError(p, "El dominio de correo contiene caracteres ilegales.");
        }
    }
    public class ContactDescriptor : CrudDescriptor<Contact>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Contacto");
            Property(p => p.Name).AsName("Nombre");

            ListProperty(p => p.Emails).Creatable().Label("Correo electrónico");
            ListProperty(p => p.Phones).Creatable().Label("Teléfonos de contacto");
        }
    }
}