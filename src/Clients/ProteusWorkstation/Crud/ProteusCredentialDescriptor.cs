/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Crud
{
    public abstract class ProteusCredentialDescriptor<T> : CrudDescriptor<T> where T : ProteusCredential, new()
    {
        protected override void DescribeModel()
        {
            Property(u => u.Name).AsName("Nombre");
            Property(u => u.ModuleBehavior)
                .Label("Seguridad predeterminada de módulo")
                .ShowInDetails()
                .Nullable();
            Property(u => u.ButtonBehavior)
                .Label("Seguridad predeterminada de menús")
                .ShowInDetails()
                .Nullable();
            Property(u => u.DefaultGranted)
                .Label("Acceso predeterminado de servicios")
                .ShowInDetails()
                .Default(SecurityFlags.Read);
            Property(u => u.DefaultRevoked)
                .Label("Bloqueo predeterminado de servicios")
                .ShowInDetails()
                .Default(SecurityFlags.None);

            ListProperty(p => p.Descriptors).Creatable().Label("Permisos explícitos").ShowInDetails();
        }
    }
}