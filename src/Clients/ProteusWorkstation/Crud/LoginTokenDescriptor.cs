/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Crud
{
    public class LoginTokenDescriptor : CrudDescriptor<LoginToken>
    {        
        protected override void DescribeModel()
        {
            FriendlyName("Token de inicio de sesión");
            OnModuleMenu(Annotations.InteractionType.Tool);

            ObjectProperty(p => p.Login).Label("Otorgado a").Required();
            Property(p => p.OnceOnly).Label("Token de único uso");
            DateProperty(p => p.Void).WithTime().Nullable().Label("Vencimiento");

            ShowAllInDetails();
            AllListColumn();

            CanCreate(false);
        }
    }
}