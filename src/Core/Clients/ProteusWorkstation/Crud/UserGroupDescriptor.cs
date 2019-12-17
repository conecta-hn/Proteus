/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models;
using System.Linq;
using System;

namespace TheXDS.Proteus.Crud
{
    public class UserGroupDescriptor : ProteusCredentialDescriptor<UserGroup>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Grupo de usuarios");

            base.DescribeModel();

            ListProperty(u => u.Children)
                .Column("Entidad", p => p.Name)
                .Selectable()
                .ShowInDetails()
                .Label("Miembros");

            BeforeSave(SetId).Then(CheckLoopback);
            CanDelete(Chk);
        }

        private void CheckLoopback(UserGroup obj)
        {
            foreach (var j in obj.Children)
            {
                if (j == obj) throw new Exception("No se puede agregar un grupo a sí mismo.");
            }
        }

        private bool Chk(UserGroup arg)
        {
            return !arg.Children.Any();
        }
        private void SetId(UserGroup obj)
        {
            if (obj.IsNew) obj.Id = Guid.NewGuid().ToString();
        }
    }
}