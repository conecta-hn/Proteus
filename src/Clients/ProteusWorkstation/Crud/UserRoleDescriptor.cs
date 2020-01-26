/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models;
using System.Linq;
using System;

namespace TheXDS.Proteus.Crud
{
    public class UserRoleDescriptor : ProteusCredentialDescriptor<UserRole>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Rol de usuario");

            base.DescribeModel();

            ListProperty(u => u.Members)
                .Column("Entidad", p => p.Name)
                .Selectable()
                .ShowInDetails()
                .Label("Miembros");

            BeforeSave(SetId);
            CanDelete(Chk);
        }

        private bool Chk(UserRole arg)
        {
            return !arg.Members.Any();
        }

        private void SetId(UserRole obj)
        {
            if (obj.IsNew) obj.Id = Guid.NewGuid().ToString();
        }
    }
}