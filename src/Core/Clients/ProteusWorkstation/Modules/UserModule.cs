/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.Proteus.Plugins;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Pages.Base;
using static TheXDS.Proteus.Annotations.InteractionType;
using TheXDS.Proteus.Pages;
using TheXDS.Proteus.Api;

namespace TheXDS.Proteus.Modules
{
    [Name("Gestión del sistema")]
    public class UserModule : UiModule
    {
        [InteractionItem, Essential, InteractionType(AdminTool), Name("Avisos")]
        public void AdminAvisos(object sender, EventArgs e)
        {
            Host.OpenPage(CrudPage.New<Aviso>());
        }
        [InteractionItem, Essential, InteractionType(AdminTool), Name("Administrar grupos")]
        public void AdminUserGroups(object sender, EventArgs e)
        {
            Host.OpenPage(CrudPage.New<UserGroup>());
        }
        [InteractionItem, Essential, InteractionType(AdminTool), Name("Administrar roles")]
        public void AdminUserRoles(object sender, EventArgs e)
        {
            Host.OpenPage(CrudPage.New<UserRole>());
        }
        [InteractionItem, Essential, InteractionType(AdminTool), Name("Administrar usuarios")]
        public void AdminUsers(object sender, EventArgs e)
        {
            Host.OpenPage(CrudPage.New<User>());
        }
        [InteractionItem, Essential, InteractionType(AdminTool), Name("Reportería genérica")]
        public void OpenReportPage(object sender, EventArgs e)
        {
            Host.OpenPage<GenericReportPage>();
        }

        [InteractionItem, Essential, InteractionType(InteractionType.Tool), Name("Borrar tokens")]
        [Description("Permite eliminar todos los tokens de inicio de sesión de la bas de datos.")]
        public async void PurgeTokens(object sender, EventArgs e)
        {
            Proteus.CommonReporter?.UpdateStatus("Eliminando todos los tokens de inicio de sesión...");
            await UserService.PurgeAllTokensAsync();
            Proteus.CommonReporter?.Done();
        }
    }
}
